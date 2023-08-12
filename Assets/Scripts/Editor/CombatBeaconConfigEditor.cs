using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(CombatBeaconConfig))]
    public sealed class CombatBeaconConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _additionalEnemies;
        private SerializedProperty _additionalEnemiesRandomDeviation;

        private SerializedProperty _additionaWaveLength;
        private SerializedProperty _additionalWaveLengthRandomDeviation;

        protected override bool DurationEditorEnabled => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _additionalEnemies = serializedObject.FindProperty("_additionalEnemies");
            _additionalEnemiesRandomDeviation = serializedObject.FindProperty("_additionalEnemiesRandomDeviation");

            _additionaWaveLength = serializedObject.FindProperty("_additionalWaveLength");
            _additionalWaveLengthRandomDeviation = serializedObject.FindProperty("_additionalWaveLengthRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_additionalEnemies, CombatBeacon.MinAdditionalEnemies, CombatBeacon.MaxAdditionalEnemies, "Additional enemies");
            EditorGUILayout.IntSlider(_additionalEnemiesRandomDeviation, 0, _additionalEnemies.intValue, "Random deviation");

            _additionalEnemiesRandomDeviation.intValue = Mathf.Clamp(_additionalEnemiesRandomDeviation.intValue, 0, _additionalEnemies.intValue);

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_additionaWaveLength, CombatBeacon.MinAdditionalWaveLength, CombatBeacon.MaxAdditionalWaveLength, "Additional wave length");
            EditorGUILayout.IntSlider(_additionalWaveLengthRandomDeviation, 0, _additionaWaveLength.intValue, "Random deviation");

            _additionalWaveLengthRandomDeviation.intValue = Mathf.Clamp(_additionalWaveLengthRandomDeviation.intValue, 0, _additionaWaveLength.intValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as CombatBeaconConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}