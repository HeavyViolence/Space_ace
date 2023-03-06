using SpaceAce.Gameplay.Movement;
using UnityEditor;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ShipMovementConfig))]
    public sealed class ShipMovementConfigEditor : MovementConfigEditor
    {
        private SerializedProperty _horizontalSpeedDuration;
        private SerializedProperty _horizontalSpeedDurationRandomDeviation;

        private SerializedProperty _horizontalSpeedTransitionDuration;
        private SerializedProperty _horizontalSpeedTransitionDurationRandomDeviation;

        private SerializedProperty _verticalSpeedDuration;
        private SerializedProperty _verticalSpeedDurationRandomDeviation;

        private SerializedProperty _verticalSpeedTranstitionDuration;
        private SerializedProperty _verticalSpeedTranstitionDurationRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _horizontalSpeedDuration = serializedObject.FindProperty("_horizontalSpeedDuration");
            _horizontalSpeedDurationRandomDeviation = serializedObject.FindProperty("_horizontalSpeedDurationRandomDeviation");

            _horizontalSpeedTransitionDuration = serializedObject.FindProperty("_horizontalSpeedTransitionDuration");
            _horizontalSpeedTransitionDurationRandomDeviation = serializedObject.FindProperty("_horizontalSpeedTransitionDurationRandomDeviation");

            _verticalSpeedDuration = serializedObject.FindProperty("_verticalSpeedDuration");
            _verticalSpeedDurationRandomDeviation = serializedObject.FindProperty("_verticalSpeedDurationRandomDeviation");

            _verticalSpeedTranstitionDuration = serializedObject.FindProperty("_verticalSpeedTranstitionDuration");
            _verticalSpeedTranstitionDurationRandomDeviation = serializedObject.FindProperty("_verticalSpeedTranstitionDurationRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_horizontalSpeedDuration,
                                   ShipMovementConfig.MinSpeedDuration,
                                   ShipMovementConfig.MaxSpeedDuration,
                                   "Horizontal speed duration");
            EditorGUILayout.Slider(_horizontalSpeedDurationRandomDeviation,
                                   0f,
                                   _horizontalSpeedDuration.floatValue,
                                   "Max random deviation");
            EditorGUILayout.Slider(_horizontalSpeedTransitionDuration,
                                   ShipMovementConfig.MinSpeedTransitionDuration,
                                   ShipMovementConfig.MaxSpeedTransitionDuration,
                                   "Transition duration");
            EditorGUILayout.Slider(_horizontalSpeedTransitionDurationRandomDeviation,
                                   0f,
                                   _horizontalSpeedTransitionDuration.floatValue,
                                   "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_verticalSpeedDuration,
                                   ShipMovementConfig.MinSpeedDuration,
                                   ShipMovementConfig.MaxSpeedDuration,
                                   "Vertical speed duration");
            EditorGUILayout.Slider(_verticalSpeedDurationRandomDeviation,
                                   0f,
                                   _verticalSpeedDuration.floatValue,
                                   "Max random deviation");
            EditorGUILayout.Slider(_verticalSpeedTranstitionDuration,
                                   ShipMovementConfig.MinSpeedTransitionDuration,
                                   ShipMovementConfig.MaxSpeedTransitionDuration,
                                   "Transition duration");
            EditorGUILayout.Slider(_verticalSpeedTranstitionDurationRandomDeviation,
                                   0f,
                                   _verticalSpeedTranstitionDuration.floatValue,
                                   "Max random deviation");

            serializedObject.ApplyModifiedProperties();
        }
    }
}