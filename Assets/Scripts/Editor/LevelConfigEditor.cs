using SpaceAce.Levelry;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(LevelConfig))]
    public sealed class LevelConfigEditor : Editor
    {
        private SerializedProperty _levelIndex;

        private SerializedProperty _enemyType;
        private SerializedProperty _difficulty;

        private SerializedProperty _crystalsReward;
        private SerializedProperty _experienceReward;

        private SerializedProperty _bossEnabled;
        private SerializedProperty _boss;

        private void OnEnable()
        {
            _levelIndex = serializedObject.FindProperty("_levelIndex");

            _enemyType = serializedObject.FindProperty("_enemyType");
            _difficulty = serializedObject.FindProperty("_difficulty");

            _crystalsReward = serializedObject.FindProperty("_crystalsReward");
            _experienceReward = serializedObject.FindProperty("_experienceReward");

            _bossEnabled = serializedObject.FindProperty("_bossEnabled");
            _boss = serializedObject.FindProperty("_boss");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.IntSlider(_levelIndex, LevelConfig.MinLevelIndex, LevelConfig.MaxLevelIndex, "Level index");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_enemyType, new GUIContent("Enemy type"));
            EditorGUILayout.PropertyField(_difficulty, new GUIContent("Difficulty"));

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_crystalsReward, LevelConfig.MinCrystalsReward, LevelConfig.MaxCrystalsReward, "Crystals reward");
            EditorGUILayout.IntSlider(_experienceReward, LevelConfig.MinExperienceReward, LevelConfig.MaxExperienceReward, "Experience reward");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_bossEnabled, new GUIContent("Enable boss"));

            if (_bossEnabled.boolValue == true)
            {
                EditorGUILayout.PropertyField(_boss, new GUIContent("Boss"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}