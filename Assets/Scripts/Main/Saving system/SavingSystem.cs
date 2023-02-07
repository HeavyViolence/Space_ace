using SpaceAce.Architecture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace SpaceAce.Main.Saving
{
    public sealed class SavingSystem : IInitializable
    {
        private const string SavesDirectory = "D:/Unity Projects/Space_ace/Saves";
        private const string MetadataFileName = "Meta";
        private const string SavesExtension = ".save";

        private const int EncryptionKeyLength = 16;

        private readonly HashSet<Type> _knownSavableDataTypes = new() { typeof(object[]) };
        private readonly HashSet<ISavable> _registeredEntities = new();

        private readonly int _metadataSaveSeed;

        private string MetadataPath => Path.Combine(SavesDirectory, MetadataFileName + SavesExtension);

        public SavingSystem(int metadataSaveSeed)
        {
            _metadataSaveSeed = metadataSaveSeed;
        }

        public bool Register(ISavable entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity), $"Attempted to register an empty {nameof(ISavable)} entity!");
            }

            if (_registeredEntities.Add(entity) == true)
            {
                Type stateType = entity.GetState().GetType();

                if (_knownSavableDataTypes.Add(stateType) == true)
                {
                    SaveMetadata();
                }

                if (TryLoadEntityState(entity, out object state) == true)
                {
                    entity.SetState(state);
                }

                entity.SavingRequested += (s, e) => SaveEntityState(entity);

                return true;
            }

            return false;
        }

        public bool Deregister(ISavable entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity), $"Attempted to deregister an empty {nameof(ISavable)} entity!");
            }

            if (_registeredEntities.Remove(entity) == true)
            {
                SaveEntityState(entity);
                entity.SavingRequested -= (s, e) => SaveEntityState(entity);

                return true;
            }

            return false;
        }

        private void SaveEntityState(ISavable entity)
        {
            using FileStream saveFileStream = new(GetSaveFilePath(entity), FileMode.Create, FileAccess.Write);

            byte[] serializedState = Serialize(entity.GetState());
            byte[] encryptionKey = GetRandomBytes(entity.GetHashCode(), EncryptionKeyLength);
            byte[] encryptedSerializedState = Encrypt(serializedState, encryptionKey);

            saveFileStream.Write(encryptedSerializedState);
            saveFileStream.Close();
        }

        private bool TryLoadEntityState(ISavable entity, out object state)
        {
            string saveFilePath = GetSaveFilePath(entity);

            if (File.Exists(saveFilePath))
            {
                using FileStream saveFileStream = new(saveFilePath, FileMode.Open, FileAccess.Read);
                using BinaryReader saveFileReader = new(saveFileStream);

                byte[] encryptedSerializedState = saveFileReader.ReadBytes((int)saveFileStream.Length);
                byte[] decryptionKey = GetRandomBytes(entity.GetHashCode(), EncryptionKeyLength);
                byte[] decryptedSerializedState = Decrypt(encryptedSerializedState, decryptionKey);

                state = Deserialize(decryptedSerializedState);

                saveFileReader.Close();
                saveFileStream.Close();

                return true;
            }
            else
            {
                state = default;

                return false;
            }
        }

        private string GetSaveFilePath(ISavable entity) => Path.Combine(SavesDirectory, entity.ID + SavesExtension);

        private void SaveMetadata()
        {
            using FileStream metadataFileStream = new(MetadataPath, FileMode.Create, FileAccess.Write);
            List<string> knownTypesNames = new(_knownSavableDataTypes.Count);

            _knownSavableDataTypes.Add(knownTypesNames.GetType());

            foreach (var type in _knownSavableDataTypes)
            {
                knownTypesNames.Add(type.FullName);
            }

            byte[] key = GetRandomBytes(_metadataSaveSeed, EncryptionKeyLength);
            byte[] metadata = Serialize(knownTypesNames);
            byte[] encryptedMetadata = Encrypt(metadata, key);

            metadataFileStream.Write(encryptedMetadata);
            metadataFileStream.Close();
        }

        private bool TryLoadMetadata()
        {
            if (File.Exists(MetadataPath))
            {
                try
                {
                    using FileStream metadataFileStream = new(MetadataPath, FileMode.Open, FileAccess.Read);
                    using BinaryReader metadataFileReader = new(metadataFileStream);

                    byte[] key = GetRandomBytes(_metadataSaveSeed, EncryptionKeyLength);
                    byte[] encryptedMetadata = metadataFileReader.ReadBytes((int)metadataFileStream.Length);
                    byte[] metadata = Decrypt(encryptedMetadata, key);
                    List<string> knownTypesNames = new();

                    _knownSavableDataTypes.Add(knownTypesNames.GetType());

                    knownTypesNames = (List<string>)Deserialize(metadata);
                    List<Type> knownTypes = new(knownTypesNames.Count);

                    foreach (var typeName in knownTypesNames)
                    {
                        knownTypes.Add(Type.GetType(typeName));
                    }

                    _knownSavableDataTypes.UnionWith(knownTypes);

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Metadata loading failed!", e);
                }
            }

            return false;
        }

        private byte[] Serialize(object input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input), "Cannot serialize an empty input!");
            }

            using MemoryStream memoryStream = new();
            DataContractSerializer serializer = new(typeof(object), _knownSavableDataTypes);

            serializer.WriteObject(memoryStream, input);

            return memoryStream.ToArray();
        }

        private object Deserialize(byte[] input)
        {
            using MemoryStream memoryStream = new(input.Length);
            DataContractSerializer deserializer = new(typeof(object), _knownSavableDataTypes);

            memoryStream.Write(input);
            memoryStream.Position = 0;

            return deserializer.ReadObject(memoryStream);
        }

        private byte[] Encrypt(byte[] input, byte[] key)
        {
            using SymmetricAlgorithm algorithm = Aes.Create();
            using ICryptoTransform encryptor = algorithm.CreateEncryptor(key, algorithm.IV);

            List<byte> output = new(algorithm.IV.Length + input.Length + algorithm.BlockSize / 8);
            byte[] encryptedInput = encryptor.TransformFinalBlock(input, 0, input.Length);

            output.AddRange(algorithm.IV);
            output.AddRange(encryptedInput);

            algorithm.Clear();

            return output.ToArray();
        }

        private byte[] Decrypt(byte[] input, byte[] key)
        {
            using SymmetricAlgorithm algorithm = Aes.Create();
            byte[] iv = input[..algorithm.IV.Length];
            using ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv);

            byte[] encryptedInput = input[algorithm.IV.Length..];
            byte[] output = decryptor.TransformFinalBlock(encryptedInput, 0, encryptedInput.Length);

            algorithm.Clear();

            return output;
        }

        private byte[] GetRandomBytes(int seed, int length)
        {
            Random random = new(seed);
            var randomBytes = new byte[length];

            random.NextBytes(randomBytes);

            return randomBytes;
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
            TryLoadMetadata();
        }

        public void OnSubscribe()
        {

        }

        public void OnUnsubscribe()
        {

        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        #endregion
    }
}