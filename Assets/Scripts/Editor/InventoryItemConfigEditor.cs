using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(InventoryItemConfig))]
    public abstract class InventoryItemConfigEditor : Editor
    {
        private SerializedProperty _rarity;

        private SerializedProperty _sellValue;
        private SerializedProperty _sellValueRandomDeviation;

        private SerializedProperty _duration;
        private SerializedProperty _durationRandomDeviation;

        protected virtual void OnEnable()
        {
            _rarity = serializedObject.FindProperty("_rarity");

            _sellValue = serializedObject.FindProperty("_sellValue");
            _sellValueRandomDeviation = serializedObject.FindProperty("_sellValueRandomDeviation");

            _duration = serializedObject.FindProperty("_duration");
            _durationRandomDeviation = serializedObject.FindProperty("_durationRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_rarity, new GUIContent("Rarity"));

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_sellValue, InventoryItem.MinSellValue, InventoryItem.MaxSellValue, "Sell value");
            EditorGUILayout.IntSlider(_sellValueRandomDeviation, 0, _sellValue.intValue, "Max random deviation");

            _sellValueRandomDeviation.intValue = Mathf.Clamp(_sellValueRandomDeviation.intValue, 0, _sellValue.intValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_duration, InventoryItem.MinDuration, InventoryItem.MaxDuration, "Duration");
            EditorGUILayout.Slider(_durationRandomDeviation, 0f, _duration.floatValue, "Max random deviation");

            _durationRandomDeviation.floatValue = Mathf.Clamp(_durationRandomDeviation.floatValue, 0f, _duration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}