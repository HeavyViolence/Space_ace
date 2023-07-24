using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(PlasmaShieldConfig))]
    public sealed class PlasmaShieldConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _armorBoost;
        private SerializedProperty _armorBoostRandomDeviation;

        private SerializedProperty _projectilesSlowdown;
        private SerializedProperty _projectilesSlowdownRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _armorBoost = serializedObject.FindProperty("_armorBoost");
            _armorBoostRandomDeviation = serializedObject.FindProperty("_armorBoostRandomDeviation");

            _projectilesSlowdown = serializedObject.FindProperty("_projectilesSlowdown");
            _projectilesSlowdownRandomDeviation = serializedObject.FindProperty("_projectilesSlowdownRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_armorBoost, PlasmaShield.MinArmorBoost, PlasmaShield.MaxArmorBoost, "Armor boost");
            EditorGUILayout.Slider(_armorBoostRandomDeviation, 0f, _armorBoost.floatValue, "Max random deviation");

            _armorBoostRandomDeviation.floatValue = Mathf.Clamp(_armorBoostRandomDeviation.floatValue, 0f, _armorBoost.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_projectilesSlowdown, PlasmaShield.MinProjectilesSlowdown, PlasmaShield.MaxProjectilesSlowdown, "Projectiles slowdown");
            EditorGUILayout.Slider(_projectilesSlowdownRandomDeviation, 0f, _projectilesSlowdown.floatValue, "Max random deviation");

            _projectilesSlowdownRandomDeviation.floatValue = Mathf.Clamp(_projectilesSlowdownRandomDeviation.floatValue, 0f, _projectilesSlowdown.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply properties"))
            {
                var config = target as PlasmaShieldConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}