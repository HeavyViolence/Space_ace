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
            EditorGUILayout.PropertyField(_rotationDirection, new GUIContent("Rotation direction"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_rpm, RotationConfig.MinRPM, RotationConfig.MaxRPM, "Rotations per minute");

            if (_rpm.floatValue > 0f)
            {
                EditorGUILayout.Slider(_rpmRandomDeviation, 0f, _rpm.floatValue, "Max random deviation");
            }
        }
    }
}