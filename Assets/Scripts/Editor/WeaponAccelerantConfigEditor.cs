using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(WeaponAccelerantConfig))]
    public sealed class WeaponAccelerantConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _ammoSpeedBoost;
        private SerializedProperty _ammoSpeedBoostRandomDeviation;

        private SerializedProperty _damageBoost;
        private SerializedProperty _damageBoostRandomDeviation;

        private SerializedProperty _cooldownIncrease;
        private SerializedProperty _cooldownIncreaseRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _ammoSpeedBoost = serializedObject.FindProperty("_ammoSpeedBoost");
            _ammoSpeedBoostRandomDeviation = serializedObject.FindProperty("_ammoSpeedBoostRandomDeviation");

            _damageBoost = serializedObject.FindProperty("_damageBoost");
            _damageBoostRandomDeviation = serializedObject.FindProperty("_damageBoostRandomDeviation");

            _cooldownIncrease = serializedObject.FindProperty("_cooldownIncrease");
            _cooldownIncreaseRandomDeviation = serializedObject.FindProperty("_cooldownIncreaseRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_ammoSpeedBoost, WeaponAccelerant.MinAmmoSpeedBoost, WeaponAccelerant.MaxAmmoSpeedBoost, "Ammo speed boost");
            EditorGUILayout.Slider(_ammoSpeedBoostRandomDeviation, 0f, _ammoSpeedBoost.floatValue, "Random deviation");

            _ammoSpeedBoostRandomDeviation.floatValue = Mathf.Clamp(_ammoSpeedBoostRandomDeviation.floatValue, 0f, _ammoSpeedBoost.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_damageBoost, WeaponAccelerant.MinDamageBoost, WeaponAccelerant.MaxDamageBoost, "Damage boost");
            EditorGUILayout.Slider(_damageBoostRandomDeviation, 0f, _damageBoost.floatValue, "Random deviation");

            _damageBoostRandomDeviation.floatValue = Mathf.Clamp(_damageBoostRandomDeviation.floatValue, 0f, _damageBoost.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_cooldownIncrease, WeaponAccelerant.MinCooldownIncrease, WeaponAccelerant.MaxCooldownIncrease, "Cooldown increase");
            EditorGUILayout.Slider(_cooldownIncreaseRandomDeviation, 0f, _cooldownIncrease.floatValue, "Random deviation");

            _cooldownIncreaseRandomDeviation.floatValue = Mathf.Clamp(_cooldownIncreaseRandomDeviation.floatValue, 0f, _cooldownIncrease.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as WeaponAccelerantConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}