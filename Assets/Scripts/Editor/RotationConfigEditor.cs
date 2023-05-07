using SpaceAce.Gameplay.Movement;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(RotationConfig))]
    public sealed class RotationConfigEditor : Editor
    {
        private SerializedProperty _rotationDirection;

        private SerializedProperty _rpm;
        private SerializedProperty _rpmRandomDeviation;

        private SerializedProperty _turningSpeed;
        private SerializedProperty _turningSpeedRandomDeviation;

        private void OnEnable()
        {
            _rotationDirection = serializedObject.FindProperty("_rotationDirection");

            _rpm = serializedObject.FindProperty("_rpm");
            _rpmRandomDeviation = serializedObject.FindProperty("_rpmRandomDeviation");

            _turningSpeed = serializedObject.FindProperty("_turningSpeed");
            _turningSpeedRandomDeviation = serializedObject.FindProperty("_turningSpeedRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_rotationDirection, new GUIContent("Rotation direction"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_rpm, RotationConfig.MinRPM, RotationConfig.MaxRPM, "Revolutions per minute");
            EditorGUILayout.Slider(_rpmRandomDeviation, 0f, _rpm.floatValue, "Max random deviation");

            _rpmRandomDeviation.floatValue = Mathf.Clamp(_rpmRandomDeviation.floatValue, 0f, _rpm.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_turningSpeed, RotationConfig.MinTurningSpeed, RotationConfig.MaxTurningSpeed, "Turning speed");
            EditorGUILayout.Slider(_turningSpeedRandomDeviation, 0f, _turningSpeed.floatValue, "Max random deviation");

            _turningSpeedRandomDeviation.floatValue = Mathf.Clamp(_turningSpeedRandomDeviation.floatValue, 0f, _turningSpeed.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as RotationConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}