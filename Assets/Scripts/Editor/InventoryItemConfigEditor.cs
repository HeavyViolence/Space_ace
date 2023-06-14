using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(InventoryItemConfig))]
    public abstract class InventoryItemConfigEditor : Editor
    {
        private SerializedProperty _rarity;

        private SerializedProperty _scrapValue;
        private SerializedProperty _scrapValueRandomDeviation;

        private SerializedProperty _duration;
        private SerializedProperty _durationRandomDeviation;

        protected virtual void OnEnable()
        {
            _rarity = serializedObject.FindProperty("_rarity");

            _scrapValue = serializedObject.FindProperty("_scrapValue");
            _scrapValueRandomDeviation = serializedObject.FindProperty("_scrapValueRandomDeviation");

            _duration = serializedObject.FindProperty("_duration");
            _durationRandomDeviation = serializedObject.FindProperty("_durationRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_rarity, new GUIContent("Rarity"));

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_scrapValue, InventoryItemConfig.MinScrapValue, InventoryItemConfig.MaxScrapValue, "Scrap value");
            EditorGUILayout.IntSlider(_scrapValueRandomDeviation, 0, _scrapValue.intValue, "Max random deviation");

            _scrapValueRandomDeviation.intValue = Mathf.Clamp(_scrapValueRandomDeviation.intValue, 0, _scrapValue.intValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_duration, InventoryItemConfig.MinDuration, InventoryItemConfig.MaxDuration, "Duration");
            EditorGUILayout.Slider(_durationRandomDeviation, 0f, _duration.floatValue, "Max random deviation");

            _durationRandomDeviation.floatValue = Mathf.Clamp(_durationRandomDeviation.floatValue, 0f, _duration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}