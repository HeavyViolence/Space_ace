using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(HomingAmmoConfig))]
    public sealed class HomingAmmoConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _homingSpeed;
        private SerializedProperty _homingSpeedRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _homingSpeed = serializedObject.FindProperty("_homingSpeed");
            _homingSpeedRandomDeviation = serializedObject.FindProperty("_homingSpeedRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_homingSpeed, HomingAmmo.MinHomingSpeed, HomingAmmo.MaxHomingSpeed, "Homing speed");
            EditorGUILayout.Slider(_homingSpeedRandomDeviation, 0f, _homingSpeed.floatValue, "Random deviation");

            _homingSpeedRandomDeviation.floatValue = Mathf.Clamp(_homingSpeedRandomDeviation.floatValue, 0f, _homingSpeed.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as HomingAmmoConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}