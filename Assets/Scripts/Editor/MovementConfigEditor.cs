using SpaceAce.Gameplay.Movement;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(MovementConfig))]
    public class MovementConfigEditor : Editor
    {
        private SerializedProperty _horizontalSpeed;
        private SerializedProperty _horizontalSpeedRandomDeviation;

        private SerializedProperty _verticalSpeed;
        private SerializedProperty _verticalSpeedRandomDeviation;

        private SerializedProperty _customBoundsEnabled;
        private SerializedProperty _upperBoundDisplacement;
        private SerializedProperty _lowerBoundDisplacement;
        private SerializedProperty _sideBoundsDisplacement;

        private SerializedProperty _collisionDamageEnabled;
        private SerializedProperty _collisionDamage;
        private SerializedProperty _collisionDamageRandomDeviation;
        private SerializedProperty _collisionAudio;
        private SerializedProperty _cameraShakeOnCollisionEnabled;

        protected virtual void OnEnable()
        {
            _horizontalSpeed = serializedObject.FindProperty("_horizontalSpeed");
            _horizontalSpeedRandomDeviation = serializedObject.FindProperty("_horizontalSpeedRandomDeviation");

            _verticalSpeed = serializedObject.FindProperty("_verticalSpeed");
            _verticalSpeedRandomDeviation = serializedObject.FindProperty("_verticalSpeedRandomDeviation");

            _customBoundsEnabled = serializedObject.FindProperty("_customBoundsEnabled");
            _upperBoundDisplacement = serializedObject.FindProperty("_upperBoundDisplacement");
            _lowerBoundDisplacement = serializedObject.FindProperty("_lowerBoundDisplacement");
            _sideBoundsDisplacement = serializedObject.FindProperty("_sideBoundsDisplacement");

            _collisionDamageEnabled = serializedObject.FindProperty("_collisionDamageEnabled");
            _collisionDamage = serializedObject.FindProperty("_collisionDamage");
            _collisionDamageRandomDeviation = serializedObject.FindProperty("_collisionDamageRandomDeviation");
            _collisionAudio = serializedObject.FindProperty("_collisionAudio");
            _cameraShakeOnCollisionEnabled = serializedObject.FindProperty("_cameraShakeOnCollisionEnabled");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Slider(_horizontalSpeed, 0f, MovementConfig.MaxSpeed, "Horizontal speed");
            
            if (_horizontalSpeed.floatValue > 0f)
            {
                EditorGUILayout.Slider(_horizontalSpeedRandomDeviation, 0f, _horizontalSpeed.floatValue, "Max random deviation");
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_verticalSpeed, 0f, MovementConfig.MaxSpeed, "Vertical speed");

            if (_verticalSpeed.floatValue > 0f)
            {
                EditorGUILayout.Slider(_verticalSpeedRandomDeviation, 0f, _verticalSpeed.floatValue, "Max random deviation");
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_customBoundsEnabled, new GUIContent("Enable custom bounds"));

            if (_customBoundsEnabled.boolValue == true)
            {
                EditorGUILayout.Slider(_upperBoundDisplacement, 0f, MovementConfig.MaxBoundDisplacement, "Upper bound displacement");
                EditorGUILayout.Slider(_lowerBoundDisplacement, 0f, MovementConfig.MaxBoundDisplacement, "Lower bound displacement");
                EditorGUILayout.Slider(_sideBoundsDisplacement, 0f, MovementConfig.MaxBoundDisplacement, "Side bounds displacement");
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_collisionDamageEnabled, new GUIContent("Enable collision damage"));

            if (_collisionDamageEnabled.boolValue == true)
            {
                EditorGUILayout.Slider(_collisionDamage, MovementConfig.MinCollisionDamage, MovementConfig.MaxCollisionDamage, "Collision damage");
                EditorGUILayout.Slider(_collisionDamageRandomDeviation, 0f, _collisionDamage.floatValue, "Max random deviation");
                EditorGUILayout.PropertyField(_collisionAudio, new GUIContent("Collision audio"));
                EditorGUILayout.PropertyField(_cameraShakeOnCollisionEnabled, new GUIContent("Camera shake"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}