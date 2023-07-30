using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(NanofuelConfig))]
    public sealed class NanofuelConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _speedIncrease;
        private SerializedProperty _speedIncreaseRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _speedIncrease = serializedObject.FindProperty("_speedIncrease");
            _speedIncreaseRandomDeviation = serializedObject.FindProperty("_speedIncreaseRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_speedIncrease, Nanofuel.MinSpeedIncrease, Nanofuel.MaxSpeedIncrease, "Speed increase");
            EditorGUILayout.Slider(_speedIncreaseRandomDeviation, 0f, _speedIncrease.floatValue, "Random deviation");

            _speedIncreaseRandomDeviation.floatValue = Mathf.Clamp(_speedIncreaseRandomDeviation.floatValue, 0f, _speedIncrease.floatValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}