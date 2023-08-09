using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(WeaponCoolantConfig))]
    public sealed class WeaponCoolantConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _cooldownReduction;
        private SerializedProperty _cooldownReductionRandomDeviation;

        private SerializedProperty _fireRateBoost;
        private SerializedProperty _fireRateBoostRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _cooldownReduction = serializedObject.FindProperty("_cooldownReduction");
            _cooldownReductionRandomDeviation = serializedObject.FindProperty("_cooldownReductionRandomDeviation");

            _fireRateBoost = serializedObject.FindProperty("_fireRateBoost");
            _fireRateBoostRandomDeviation = serializedObject.FindProperty("_fireRateBoostRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_cooldownReduction, WeaponCoolant.MinCooldownReduction, WeaponCoolant.MaxCooldownReduction, "Cooldown reduction");
            EditorGUILayout.Slider(_cooldownReductionRandomDeviation, 0f, _cooldownReduction.floatValue, "Random deviation");

            _cooldownReductionRandomDeviation.floatValue = Mathf.Clamp(_cooldownReductionRandomDeviation.floatValue, 0f, _cooldownReduction.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_fireRateBoost, WeaponCoolant.MinFireRateBoost, WeaponCoolant.MaxFirerateBoost, "Fire rate boost");
            EditorGUILayout.Slider(_fireRateBoostRandomDeviation, 0f, _fireRateBoost.floatValue, "Random deviation");

            _fireRateBoostRandomDeviation.floatValue = Mathf.Clamp(_fireRateBoostRandomDeviation.floatValue, 0f, _fireRateBoost.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as WeaponCoolantConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}