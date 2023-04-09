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

        private void OnEnable()
        {
            _rotationDirection = serializedObject.FindProperty("_rotationDirection");

            _rpm = serializedObject.FindProperty("_rpm");
            _rpmRandomDeviation = serializedObject.FindProperty("_rpmRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_rotationDirection, new GUIContent("Rotation direction"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_rpm, RotationConfig.MinRPM, RotationConfig.MaxRPM, "Rotations per minute");
            EditorGUILayout.Slider(_rpmRandomDeviation, 0f, _rpm.floatValue, "Max random deviation");

            _rpmRandomDeviation.floatValue = Mathf.Clamp(_rpmRandomDeviation.floatValue, 0f, _rpm.floatValue);

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