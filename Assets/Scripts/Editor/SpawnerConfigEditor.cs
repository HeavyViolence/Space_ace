using SpaceAce.Gameplay.Spawning;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(SpawnerConfig))]
    public sealed class SpawnerConfigEditor : Editor
    {
        private SerializedProperty _spawnableEntities;

        private SerializedProperty _firstSpawnDelay;
        private SerializedProperty _firstSpawnDelayRandomDeviation;

        private SerializedProperty _spawnDelay;
        private SerializedProperty _spawnDelayRandomDeviation;

        private SerializedProperty _uniqueEntitiesInWave;
        private SerializedProperty _uniqueEntitiesInWaveRandomDeviation;

        private SerializedProperty _waveLength;
        private SerializedProperty _waveLengthRandomDeviation;

        private SerializedProperty _spawnForever;

        private SerializedProperty _amountToSpawn;
        private SerializedProperty _amountToSpawnRandomDeviation;

        private SerializedProperty _haltUntilClear;

        private SerializedProperty _amplificationConfig;

        private SpawnerConfig _target;

        private void OnEnable()
        {
            _spawnableEntities = serializedObject.FindProperty("_spawnableEntities");

            _firstSpawnDelay = serializedObject.FindProperty("_firstSpawnDelay");
            _firstSpawnDelayRandomDeviation = serializedObject.FindProperty("_firstSpawnDelayRandomDeviation");

            _spawnDelay = serializedObject.FindProperty("_spawnDelay");
            _spawnDelayRandomDeviation = serializedObject.FindProperty("_spawnDelayRandomDeviation");

            _uniqueEntitiesInWave = serializedObject.FindProperty("_uniqueEntitiesInWave");
            _uniqueEntitiesInWaveRandomDeviation = serializedObject.FindProperty("_uniqueEntitiesInWaveRandomDeviation");

            _waveLength = serializedObject.FindProperty("_waveLength");
            _waveLengthRandomDeviation = serializedObject.FindProperty("_waveLengthRandomDeviation");

            _spawnForever = serializedObject.FindProperty("_spawnForever");

            _amountToSpawn = serializedObject.FindProperty("_amountToSpawn");
            _amountToSpawnRandomDeviation = serializedObject.FindProperty("_amountToSpawnRandomDeviation");

            _haltUntilClear = serializedObject.FindProperty("_haltUntilClear");

            _amplificationConfig = serializedObject.FindProperty("_amplificationConfig");

            _target = (SpawnerConfig)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_spawnableEntities, new GUIContent("Entities to spawn"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_firstSpawnDelay, SpawnerConfig.MinFirstSpawnDelay, SpawnerConfig.MaxFirstSpawnDelay, "First spawn delay");
            EditorGUILayout.Slider(_firstSpawnDelayRandomDeviation, 0f, _firstSpawnDelay.floatValue, "Max random deviation");

            _firstSpawnDelayRandomDeviation.floatValue = Mathf.Clamp(_firstSpawnDelayRandomDeviation.floatValue, 0f, _firstSpawnDelay.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_spawnDelay, SpawnerConfig.MinSpawnDelay, SpawnerConfig.MaxSpawnDelay, "Spawn delay");
            EditorGUILayout.Slider(_spawnDelayRandomDeviation, 0f, _spawnDelay.floatValue, "Max random deviation");

            _spawnDelayRandomDeviation.floatValue = Mathf.Clamp(_spawnDelayRandomDeviation.floatValue, 0f, _spawnDelay.floatValue);

            if (_target.UniqueEntitiesAmount > 1)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.IntSlider(_uniqueEntitiesInWave, SpawnerConfig.MinUniqueEntitiesInWawe, _target.UniqueEntitiesAmount, "Unique entities in wave");
                EditorGUILayout.IntSlider(_uniqueEntitiesInWaveRandomDeviation, 0, _uniqueEntitiesInWave.intValue, "Max random deviation");

                _uniqueEntitiesInWaveRandomDeviation.intValue = Mathf.Clamp(_uniqueEntitiesInWaveRandomDeviation.intValue, 0, _uniqueEntitiesInWave.intValue);
            }
            else
            {
                _uniqueEntitiesInWave.intValue = 1;
                _uniqueEntitiesInWaveRandomDeviation.intValue = 0;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_waveLength, SpawnerConfig.MinWaveLength, SpawnerConfig.MaxWaveLength, "Wave length");
            EditorGUILayout.IntSlider(_waveLengthRandomDeviation, 0, _waveLength.intValue, "Max random devitaion");

            _waveLengthRandomDeviation.intValue = Mathf.Clamp(_waveLengthRandomDeviation.intValue, 0, _waveLength.intValue);

            EditorGUILayout.Separator();

            if (_spawnForever.boolValue == false)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.IntSlider(_amountToSpawn, SpawnerConfig.MinAmountToSpawn, SpawnerConfig.MaxAmountToSpawn, "Amount to spawn");
                EditorGUILayout.IntSlider(_amountToSpawnRandomDeviation, 0, _amountToSpawn.intValue, "Max random deviation");

                _amountToSpawnRandomDeviation.intValue = Mathf.Clamp(_amountToSpawnRandomDeviation.intValue, 0, _amountToSpawn.intValue);
            }

            EditorGUILayout.PropertyField(_spawnForever, new GUIContent("Spawn forever"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_haltUntilClear, new GUIContent("Halt until clear"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_amplificationConfig, new GUIContent("Amplification config"));

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                _target.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}