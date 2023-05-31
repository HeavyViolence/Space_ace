using SpaceAce.Gameplay.Loot;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(Loot))]
    public sealed class LootEditor : Editor
    {
        private SerializedProperty _lootConfig;

        private SerializedProperty _enableAmplifiedLoot;
        private SerializedProperty _amplifiedLootConfig;

        private void OnEnable()
        {
            _lootConfig = serializedObject.FindProperty("_lootConfig");

            _enableAmplifiedLoot = serializedObject.FindProperty("_enableAmplifiedLoot");
            _amplifiedLootConfig = serializedObject.FindProperty("_amplifiedLootConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_lootConfig, new GUIContent("Default loot"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_enableAmplifiedLoot, new GUIContent("Enable amplified loot"));

            if (_enableAmplifiedLoot.boolValue == true)
            {
                EditorGUILayout.PropertyField(_amplifiedLootConfig, new GUIContent("Amplified loot"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}