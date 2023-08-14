using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(OreConfig))]
    public sealed class OreConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _density;
        private SerializedProperty _densityRandomDeviation;

        protected override bool DurationEditorEnabled => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _density = serializedObject.FindProperty("_density");
            _densityRandomDeviation = serializedObject.FindProperty("_densityRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_density, Ore.MinDensity, Ore.MaxDensity, "Density");
            EditorGUILayout.Slider(_densityRandomDeviation, 0f, _density.floatValue, "Random deviation");

            _densityRandomDeviation.floatValue = Mathf.Clamp(_densityRandomDeviation.floatValue, 0f, _density.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as OreConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}