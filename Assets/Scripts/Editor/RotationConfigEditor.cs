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

        private SerializedProperty _targetSeekingSpeed;
        private SerializedProperty _targetSeekingSpeedRandomDeviation;

        private void OnEnable()
        {
            _rotationDirection = serializedObject.FindProperty("_rotationDirection");

            _rpm = serializedObject.FindProperty("_rpm");
            _rpmRandomDeviation = serializedObject.FindProperty("_rpmRandomDeviation");

            _targetSeekingSpeed = serializedObject.FindProperty("_targetSeekingSpeed");
            _targetSeekingSpeedRandomDeviation = serializedObject.FindProperty("_targetSeekingSpeedRandomDeviation");
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
            EditorGUILayout.Slider(_targetSeekingSpeed, RotationConfig.MinTurningSpeed, RotationConfig.MaxTurningSpeed, "Target seeking speed");
            EditorGUILayout.Slider(_targetSeekingSpeedRandomDeviation, 0f, _targetSeekingSpeed.floatValue, "Max random deviation");

            _targetSeekingSpeedRandomDeviation.floatValue = Mathf.Clamp(_targetSeekingSpeedRandomDeviation.floatValue, 0f, _targetSeekingSpeed.floatValue);

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