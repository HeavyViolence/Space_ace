using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(HardwareScannerConfig))]
    public sealed class HardwareScannerConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _hardwareSpawnProbabilityIncrease;
        private SerializedProperty _hardwareSpawnProbabilityIncreaseRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _hardwareSpawnProbabilityIncrease = serializedObject.FindProperty("_hardwareSpawnProbabilityIncrease");
            _hardwareSpawnProbabilityIncreaseRandomDeviation = serializedObject.FindProperty("_hardwareSpawnProbabilityIncreaseRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();

            EditorGUILayout.Slider(_hardwareSpawnProbabilityIncrease,
                                   HardwareScanner.MinHardawareSpawnProbabilityIncrease,
                                   HardwareScanner.MaxHardwareSpawnProbabilityIncrease,
                                   "Hardware spawn probability increase");

            EditorGUILayout.Slider(_hardwareSpawnProbabilityIncreaseRandomDeviation,
                                   0f,
                                   _hardwareSpawnProbabilityIncrease.floatValue,
                                   "Random deviation");

            _hardwareSpawnProbabilityIncreaseRandomDeviation.floatValue = Mathf.Clamp(_hardwareSpawnProbabilityIncreaseRandomDeviation.floatValue,
                                                                                      0f,
                                                                                      _hardwareSpawnProbabilityIncrease.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as HardwareScannerConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}