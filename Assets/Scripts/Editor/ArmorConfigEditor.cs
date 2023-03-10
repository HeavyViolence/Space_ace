using SpaceAce.Gameplay.Damageables;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ArmorConfig))]
    public sealed class ArmorConfigEditor : Editor
    {
        private SerializedProperty _armorEnabled;

        private SerializedProperty _armorValue;
        private SerializedProperty _armorValueRandomDeviation;

        private void OnEnable()
        {
            _armorEnabled = serializedObject.FindProperty("_armorEnabled");

            _armorValue = serializedObject.FindProperty("_armorValue");
            _armorValueRandomDeviation = serializedObject.FindProperty("_armorValueRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_armorEnabled, new GUIContent("Enable armor"));

            if (_armorEnabled.boolValue == true)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Slider(_armorValue, ArmorConfig.MinArmor, ArmorConfig.MaxArmor, "Armor");
                EditorGUILayout.Slider(_armorValueRandomDeviation, 0f, _armorValue.floatValue, "Max random deviation");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}