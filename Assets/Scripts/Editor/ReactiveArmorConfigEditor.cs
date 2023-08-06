using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ReactiveArmorConfig))]
    public sealed class ReactiveArmorConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _movementSlowdown;
        private SerializedProperty _movementSlowdownRandomDeviation;

        private SerializedProperty _healthIncrease;
        private SerializedProperty _healthDecreaseRandomDeviation;

        private SerializedProperty _damageToArmorConversionRate;
        private SerializedProperty _damageToArmorConversionRateRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _movementSlowdown = serializedObject.FindProperty("_movementSlowdown");
            _movementSlowdownRandomDeviation = serializedObject.FindProperty("_movementSlowdownRandomDeviation");

            _healthIncrease = serializedObject.FindProperty("_healthIncrease");
            _healthDecreaseRandomDeviation = serializedObject.FindProperty("_healthIncreaseRandomDeviation");

            _damageToArmorConversionRate = serializedObject.FindProperty("_damageToArmorConversionRate");
            _damageToArmorConversionRateRandomDeviation = serializedObject.FindProperty("_damageToArmorConversionRateRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_movementSlowdown, ReactiveArmor.MinMovementSlowdown, ReactiveArmor.MaxMovementSlowdown, "Movement slowdown");
            EditorGUILayout.Slider(_movementSlowdownRandomDeviation, 0f, _movementSlowdown.floatValue, "Random deviation");

            _movementSlowdownRandomDeviation.floatValue = Mathf.Clamp(_movementSlowdownRandomDeviation.floatValue, 0f, _movementSlowdown.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_healthIncrease, ReactiveArmor.MinHealthIncrease, ReactiveArmor.MaxHealthIncrease, "Health increase");
            EditorGUILayout.Slider(_healthDecreaseRandomDeviation, 0f, _healthIncrease.floatValue, "Random deviation");

            _healthDecreaseRandomDeviation.floatValue = Mathf.Clamp(_healthDecreaseRandomDeviation.floatValue, 0f, _healthIncrease.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_damageToArmorConversionRate,
                                   ReactiveArmor.MinDamageToArmorConversionRate,
                                   ReactiveArmor.MaxDamageToArmorConversionRate,
                                   "Damage to armor conversion rate");
            EditorGUILayout.Slider(_damageToArmorConversionRateRandomDeviation, 0f, _damageToArmorConversionRate.floatValue, "Random deviation");

            _damageToArmorConversionRateRandomDeviation.floatValue = Mathf.Clamp(_damageToArmorConversionRateRandomDeviation.floatValue,
                                                                                 0f,
                                                                                 _damageToArmorConversionRate.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as ReactiveArmorConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}