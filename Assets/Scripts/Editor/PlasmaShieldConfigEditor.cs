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

        protected override void OnEnable()
        {
            base.OnEnable();

            _armorBoost = serializedObject.FindProperty("_armorBoost");
            _armorBoostRandomDeviation = serializedObject.FindProperty("_armorBoostRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_armorBoost, PlasmaShieldConfig.MinArmorBoost, PlasmaShieldConfig.MaxArmorBoost, "Armor boost");
            EditorGUILayout.Slider(_armorBoostRandomDeviation, 0f, _armorBoost.floatValue, "Max random deviation");

            _armorBoostRandomDeviation.floatValue = Mathf.Clamp(_armorBoostRandomDeviation.floatValue, 0f, _armorBoost.floatValue);

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