using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ArmorDiffuserConfig))]
    public sealed class ArmorDiffuserConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _armorReduction;
        private SerializedProperty _armorReductionRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _armorReduction = serializedObject.FindProperty("_armorReduction");
            _armorReductionRandomDeviation = serializedObject.FindProperty("_armorReductionRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_armorReduction, ArmorDiffuser.MinArmorReduction, ArmorDiffuser.MaxArmorReduction, "Armor reduction");
            EditorGUILayout.Slider(_armorReductionRandomDeviation, 0f, _armorReduction.floatValue, "Random deviation");

            _armorReductionRandomDeviation.floatValue = Mathf.Clamp(_armorReductionRandomDeviation.floatValue, 0f, _armorReduction.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as ArmorDiffuserConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}