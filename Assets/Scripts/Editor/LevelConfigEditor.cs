using SpaceAce.Levels;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(LevelConfig))]
    public sealed class LevelConfigEditor : Editor
    {
        private SerializedProperty _levelIndex;

        private SerializedProperty _crystalsReward;
        private SerializedProperty _experienceReward;

        private SerializedProperty _bossEnabled;
        private SerializedProperty _boss;

        private SerializedProperty _enemySpawnerConfig;
        private SerializedProperty _meteorSpawnerConfig;
        private SerializedProperty _spaceDebrisSpawnerConfig;

        private void OnEnable()
        {
            _levelIndex = serializedObject.FindProperty("_levelIndex");

            _crystalsReward = serializedObject.FindProperty("_crystalsReward");
            _experienceReward = serializedObject.FindProperty("_experienceReward");

            _bossEnabled = serializedObject.FindProperty("_bossEnabled");
            _boss = serializedObject.FindProperty("_boss");

            _enemySpawnerConfig = serializedObject.FindProperty("_enemySpawnerConfig");
            _meteorSpawnerConfig = serializedObject.FindProperty("_meteorSpawnerConfig");
            _spaceDebrisSpawnerConfig = serializedObject.FindProperty("_spaceDebrisSpawnerConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.IntSlider(_levelIndex, LevelConfig.MinLevelIndex, LevelConfig.MaxLevelIndex, "Level index");

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_crystalsReward, LevelConfig.MinCrystalsReward, LevelConfig.MaxCrystalsReward, "Crystals reward");
            EditorGUILayout.IntSlider(_experienceReward, LevelConfig.MinExperienceReward, LevelConfig.MaxExperienceReward, "Experience reward");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_bossEnabled, new GUIContent("Enable boss"));

            if (_bossEnabled.boolValue == true)
            {
                EditorGUILayout.PropertyField(_boss, new GUIContent("Boss"));
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_enemySpawnerConfig, new GUIContent("Enemy spawner config"));
            EditorGUILayout.PropertyField(_meteorSpawnerConfig, new GUIContent("Meteor spawner config"));
            EditorGUILayout.PropertyField(_spaceDebrisSpawnerConfig, new GUIContent("Space debris spawner config"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}