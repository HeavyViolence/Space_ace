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

        private SerializedProperty _turningRadius;
        private SerializedProperty _turningRadiusRandomDeviation;

        private void OnEnable()
        {
            _rotationDirection = serializedObject.FindProperty("_rotationDirection");

            _rpm = serializedObject.FindProperty("_rpm");
            _rpmRandomDeviation = serializedObject.FindProperty("_rpmRandomDeviation");

            _turningRadius = serializedObject.FindProperty("_turningRadius");
            _turningRadiusRandomDeviation = serializedObject.FindProperty("_turningRadiusRandomDeviation");
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
            EditorGUILayout.Slider(_turningRadius, RotationConfig.MinTurningRadius, RotationConfig.MaxTurningRadius, "Turning radius");
            EditorGUILayout.Slider(_turningRadiusRandomDeviation, 0f, _turningRadius.floatValue, "Max random deviation");

            _turningRadiusRandomDeviation.floatValue = Mathf.Clamp(_turningRadiusRandomDeviation.floatValue, 0f, _turningRadius.floatValue);

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