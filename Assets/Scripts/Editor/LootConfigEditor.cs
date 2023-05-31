using SpaceAce.Gameplay.Loot;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(LootConfig))]
    public sealed class LootConfigEditor : Editor
    {
        private SerializedProperty _loot;

        private SerializedProperty _lootAmount;
        private SerializedProperty _lootAmountRandomDeviation;

        private SerializedProperty _spawnProbability;

        private void OnEnable()
        {
            _loot = serializedObject.FindProperty("_loot");

            _lootAmount = serializedObject.FindProperty("_lootAmount");
            _lootAmountRandomDeviation = serializedObject.FindProperty("_lootAmountRandomDeviation");

            _spawnProbability = serializedObject.FindProperty("_spawnProbability");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_loot, new GUIContent("Loot"));

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_lootAmount, LootConfig.MinLootAmount, LootConfig.MaxLootAmount, "Loot amount");
            EditorGUILayout.IntSlider(_lootAmountRandomDeviation, 0, _lootAmount.intValue, "Max random deviation");

            _lootAmountRandomDeviation.intValue = Mathf.Clamp(_lootAmountRandomDeviation.intValue, 0, _lootAmount.intValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_spawnProbability, 0f, 1f, "Spawn probability");
            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as LootConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}