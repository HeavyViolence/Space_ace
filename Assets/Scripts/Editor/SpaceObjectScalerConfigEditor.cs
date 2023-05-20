using SpaceAce.Auxiliary;
using UnityEditor;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(SpaceObjectScalerConfig))]
    public sealed class SpaceObjectScalerConfigEditor : Editor
    {
        private SerializedProperty _minScale, _maxScale;
        private float _minValue, _maxValue;

        private void OnEnable()
        {
            _minScale = serializedObject.FindProperty("_minScale");
            _maxScale = serializedObject.FindProperty("_maxScale");

            _minValue = _minScale.floatValue;
            _maxValue = _maxScale.floatValue;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.MinMaxSlider("Scale range", ref _minValue, ref _maxValue, SpaceObjectScalerConfig.MinScale, SpaceObjectScalerConfig.MaxScale);

            _minScale.floatValue = _minValue;
            _maxScale.floatValue = _maxValue;

            EditorGUILayout.HelpBox($"Min = {_minScale.floatValue}, Max = {_maxScale.floatValue}", MessageType.None);

            serializedObject.ApplyModifiedProperties();
        }
    }
}