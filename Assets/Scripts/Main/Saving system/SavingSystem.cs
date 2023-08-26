using SpaceAce.Architecture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SpaceAce.Main.Saving
{
    public sealed class SavingSystem : IGameService
    {
        private const string SavesDirectory = "D:/Unity Projects/Space_ace/Saves";
        private const string SavesExtension = ".save";

        private const int EncryptionKeyLength = 16;

        private readonly HashSet<ISavable> _registeredEntities = new();

        public SavingSystem() { }

        public bool Register(ISavable entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            if (_registeredEntities.Add(entity) == true)
            {
                if (TryLoadEntityState(entity, out string state) == true) entity.SetState(state);

                entity.SavingRequested += (s, e) => SaveEntityState(entity);

                return true;
            }

            return false;
        }

        public bool Deregister(ISavable entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

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
            string state = entity.GetState();
            byte[] serializedState = Encoding.UTF8.GetBytes(state);
            byte[] encryptionKey = GenerateKey(entity.GetHashCode(), EncryptionKeyLength);
            byte[] encryptedSerializedState = Encrypt(serializedState, encryptionKey);
            string encryptedSerializedStateAsBase64 = Convert.ToBase64String(encryptedSerializedState);

            string savePath = GetSavePath(entity);
            File.WriteAllText(savePath, encryptedSerializedStateAsBase64);
        }

        private bool TryLoadEntityState(ISavable entity, out string state)
        {
            string savePath = GetSavePath(entity);

            if (File.Exists(savePath))
            {
                try
                {
                    string encryptedSerializedStateAsBase64 = File.ReadAllText(savePath);
                    byte[] encryptedSerializedState = Convert.FromBase64String(encryptedSerializedStateAsBase64);
                    byte[] decryptionKey = GenerateKey(entity.GetHashCode(), EncryptionKeyLength);
                    byte[] decryptedSerializedState = Decrypt(encryptedSerializedState, decryptionKey);
                    UTF8Encoding utf8 = new(true, true);

                    state = utf8.GetString(decryptedSerializedState);
                    return true;
                }
                catch (Exception)
                {
                    state = string.Empty;
                    return false;
                }
            }

            state = string.Empty;
            return false;
        }

        private string GetSavePath(ISavable entity) => Path.Combine(SavesDirectory, entity.ID + SavesExtension);

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

        private byte[] GenerateKey(int seed, int length)
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