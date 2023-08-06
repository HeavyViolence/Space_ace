using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(StasisFieldConfig))]
    public sealed class StasisFieldConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _slowdown;
        private SerializedProperty _slowdownRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _slowdown = serializedObject.FindProperty("_slowdown");
            _slowdownRandomDeviation = serializedObject.FindProperty("_slowdownRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_slowdown, StasisField.MinSlowdown, StasisField.MaxSlowdown, "Slowdown");
            EditorGUILayout.Slider(_slowdownRandomDeviation, 0f, _slowdown.floatValue, "Random deviation");

            _slowdownRandomDeviation.floatValue = Mathf.Clamp(_slowdownRandomDeviation.floatValue, 0f, _slowdown.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as StasisFieldConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}