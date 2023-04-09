using SpaceAce.Gameplay.Damageables;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ArmorConfig))]
    public sealed class ArmorConfigEditor : Editor
    {
        private SerializedProperty _armorEnabled;

        private SerializedProperty _armor;
        private SerializedProperty _armorRandomDeviation;

        private void OnEnable()
        {
            _armorEnabled = serializedObject.FindProperty("_armorEnabled");

            _armor = serializedObject.FindProperty("_armor");
            _armorRandomDeviation = serializedObject.FindProperty("_armorRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_armorEnabled, new GUIContent("Enable armor"));

            if (_armorEnabled.boolValue == true)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Slider(_armor, ArmorConfig.MinArmor, ArmorConfig.MaxArmor, "Armor");
                EditorGUILayout.Slider(_armorRandomDeviation, 0f, _armor.floatValue, "Max random deviation");

                _armorRandomDeviation.floatValue = Mathf.Clamp(_armorRandomDeviation.floatValue, 0f, _armor.floatValue);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as ArmorConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}