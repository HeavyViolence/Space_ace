using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(OreScannerConfig))]
    public sealed class OreScannerConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _oreSpawnProbabilityIncrease;
        private SerializedProperty _oreSpawnProbabilityIncreaseRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _oreSpawnProbabilityIncrease = serializedObject.FindProperty("_oreSpawnProbabilityIncrease");
            _oreSpawnProbabilityIncreaseRandomDeviation = serializedObject.FindProperty("_oreSpawnProbabilityIncreaseRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();

            EditorGUILayout.Slider(_oreSpawnProbabilityIncrease,
                                   OreScanner.MinOreSpawnProbabilityIncrease,
                                   OreScanner.MaxOreSpawnProbabilityIncrease,
                                   "Ore spawn probability increase");

            EditorGUILayout.Slider(_oreSpawnProbabilityIncreaseRandomDeviation,
                                   0f,
                                   _oreSpawnProbabilityIncrease.floatValue,
                                   "Random deviation");

            _oreSpawnProbabilityIncreaseRandomDeviation.floatValue = Mathf.Clamp(_oreSpawnProbabilityIncreaseRandomDeviation.floatValue,
                                                                                 0f,
                                                                                 _oreSpawnProbabilityIncrease.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as OreScannerConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}