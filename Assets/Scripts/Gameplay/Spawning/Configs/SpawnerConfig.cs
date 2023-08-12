using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Main.ObjectPooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Spawning
{
    [CreateAssetMenu(fileName = "Spawner config", menuName = "Space ace/Configs/Spawning/Spawner config")]
    public sealed class SpawnerConfig : ScriptableObject
    {
        public const float MinFirstSpawnDelay = 1f;
        public const float MaxFirstSpawnDelay = 30f;
        public const float DefaultFirstSpawnDelay = 5f;

        public const float MinSpawnDelay = 1f;
        public const float MaxSpawnDelay = 30f;
        public const float DefaultSpawnDelay = 5f;

        public const int MinUniqueEntitiesInWawe = 1;

        public const int MinWaveLength = 1;
        public const int MaxWaveLength = 10;

        public const int MinAmountToSpawn = 1;
        public const int MaxAmountToSpawn = 200;

        public const float MaxEscapeDelta = 10f;

        [SerializeField] private List<ObjectPoolEntry> _spawnableEntities = new();

        [SerializeField] private float _firstSpawnDelay = DefaultFirstSpawnDelay;
        [SerializeField] private float _firstSpawnDelayRandomDeviation = 0f;

        [SerializeField] private float _spawnDelay = DefaultSpawnDelay;
        [SerializeField] private float _spawnDelayRandomDeviation = 0f;

        [SerializeField] private int _uniqueEntitiesInWave = MinUniqueEntitiesInWawe;
        [SerializeField] private int _uniqueEntitiesInWaveRandomDeviation = 0;

        [SerializeField] private int _waveLength = MinWaveLength;
        [SerializeField] private int _waveLengthRandomDeviation = 0;

        [SerializeField] private bool _spawnForever = false;

        [SerializeField] private int _amountToSpawn = MinAmountToSpawn;
        [SerializeField] private int _amountToSpawnRandomDeviation = 0;

        [SerializeField] private bool _haltUntilClear = false;

        [SerializeField] private bool _enableAmplification = false;
        [SerializeField] private AmplificationConfig _amplificationConfig;

        [SerializeField] private float _escapeDelta = 0f;

        public RangedFloat FirstSpawnDelay { get; private set; }

        public RangedFloat SpawnDelay { get; private set; }

        public RangedInt UniqueEntitiesInWave { get; private set; }

        public RangedInt WaveLength { get; private set; }

        public bool SpawnForever => _spawnForever;

        public RangedInt AmountToSpawn { get; private set; }

        public bool HaltUntilClear => _haltUntilClear;

        public int UniqueEntitiesAmount => _spawnableEntities.Count;

        public bool AmplificationEnabled => _enableAmplification;
        public AmplificationConfig AmplificationConfig => AmplificationEnabled ? _amplificationConfig : null;

        public float EscapeDelta => _escapeDelta;

        private void OnEnable()
        {
            if (UniqueEntitiesAmount > 0) ApplySettings();
        }

        public void ApplySettings()
        {
            if (UniqueEntitiesAmount > 0)
            {
                FirstSpawnDelay = new(_firstSpawnDelay, _firstSpawnDelayRandomDeviation);
                SpawnDelay = new(_spawnDelay, _spawnDelayRandomDeviation);
                UniqueEntitiesInWave = new(_uniqueEntitiesInWave, _uniqueEntitiesInWaveRandomDeviation, 1, _spawnableEntities.Count);
                WaveLength = new(_waveLength, _waveLengthRandomDeviation, 1, MaxWaveLength * 2);
                AmountToSpawn = SpawnForever ? RangedInt.Max : new(_amountToSpawn, _amountToSpawnRandomDeviation);
            }
            else
            {
                throw new ArgumentNullException(nameof(_spawnableEntities));
            }
        }

        public void EnsureNecessaryObjectPoolsExistence()
        {
            foreach (var entity in _spawnableEntities) entity.EnsureObjectPoolExistence();
        }

        public IEnumerable<(string anchorName, float spawnDelay)> GetProceduralWave(int additionalEntitiesAmount = 0)
        {
            if (additionalEntitiesAmount < 0) throw new ArgumentOutOfRangeException(nameof(additionalEntitiesAmount));

            int waveLength = WaveLength.RandomValue + additionalEntitiesAmount;

            List<(string anchorname, float spawnDelay)> proceduralWave = new(waveLength);
            List<string> entitiesToSpawnAnchorNames = GetEntitiesToSpawnAnchorNames(UniqueEntitiesInWave.RandomValue);
            List<float> spawnProbabilities = GetSpawnProbabilities(entitiesToSpawnAnchorNames.Count);

            for (int i = 0; i < waveLength; i++)
            {
                for (int j = 0; j < spawnProbabilities.Count; j++)
                {
                    if (AuxMath.Random < spawnProbabilities[j])
                    {
                        int randomAnchorNameIndex = UnityEngine.Random.Range(0, entitiesToSpawnAnchorNames.Count);

                        string anchorName = entitiesToSpawnAnchorNames[randomAnchorNameIndex];
                        float spawnDelay = i == 0 ? FirstSpawnDelay.RandomValue : SpawnDelay.RandomValue;

                        proceduralWave.Add((anchorName, spawnDelay));

                        break;
                    }
                }
            }

            return proceduralWave;
        }

        private List<string> GetEntitiesToSpawnAnchorNames(int uniqueEntitiesInWave)
        {
            List<string> entitiesToSpawnAnchorNames = new(uniqueEntitiesInWave);
            var entitiesToSpawnIndices = AuxMath.GetRandomNumbersWithoutRepetition(0, UniqueEntitiesAmount, uniqueEntitiesInWave);

            foreach (var index in entitiesToSpawnIndices)
            {
                string key = _spawnableEntities[index].AnchorName;
                entitiesToSpawnAnchorNames.Add(key);
            }

            return entitiesToSpawnAnchorNames;
        }

        private List<float> GetSpawnProbabilities(int uniqueEintitiesInWave)
        {
            List<float> spawnProbabilities = new(uniqueEintitiesInWave);

            for (int i = 0; i < uniqueEintitiesInWave; i++) spawnProbabilities.Add(AuxMath.Random);

            spawnProbabilities.Sort(CompareProbabilitiesByValueAscending);
            float normalizationFactor = 1f / spawnProbabilities[^1];

            for (int i = 0; i < uniqueEintitiesInWave; i++) spawnProbabilities[i] *= normalizationFactor;

            return spawnProbabilities;
        }

        private int CompareProbabilitiesByValueAscending(float p1, float p2)
        {
            if (p1 < p2) return -1;
            if (p1 > p2) return 1;

            return 0;
        }
    }
}