using SpaceAce.Main;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(LevelConfig))]
    public sealed class LevelConfigEditor : Editor
    {
        private SerializedProperty _enemyType;
        private SerializedProperty _difficulty;

        private void OnEnable()
        {
            _enemyType = serializedObject.FindProperty("_enemyType");
            _difficulty = serializedObject.FindProperty("_difficulty");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_enemyType, new GUIContent("Enemy type"));
            EditorGUILayout.PropertyField(_difficulty, new GUIContent("Difficulty"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}