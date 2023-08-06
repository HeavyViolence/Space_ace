using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(AntimatterAmmoConfig))]
    public sealed class AntimatterAmmoConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _consecutiveDamageFactor;
        private SerializedProperty _consecutiveDamageFactorRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _consecutiveDamageFactor = serializedObject.FindProperty("_consecutiveDamageFactor");
            _consecutiveDamageFactorRandomDeviation = serializedObject.FindProperty("_consecutiveDamageFactorRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_consecutiveDamageFactor,
                                   AntimatterAmmo.MinConsecutiveDamageFactor,
                                   AntimatterAmmo.MaxConsecutiveDamageFactor,
                                   "Consecutive damage factor");
            EditorGUILayout.Slider(_consecutiveDamageFactorRandomDeviation,
                                   0f,
                                   _consecutiveDamageFactor.floatValue,
                                   "Random deviation");

            _consecutiveDamageFactorRandomDeviation.floatValue = Mathf.Clamp(_consecutiveDamageFactorRandomDeviation.floatValue,
                                                                             0f,
                                                                             _consecutiveDamageFactor.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as AntimatterAmmoConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}