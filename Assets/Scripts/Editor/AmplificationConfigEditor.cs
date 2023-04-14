using SpaceAce.Gameplay.Amplifications;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(AmplificationConfig))]
    public sealed class AmplificationConfigEditor : Editor
    {
        private SerializedProperty _amplifiedEntitySpawnAudio;

        private SerializedProperty _amplificationFactor;
        private SerializedProperty _amplificationFactorRandomDeviation;

        private SerializedProperty _amplificationProbability;
        private SerializedProperty _amplificationProbabilityRandomDeviation;

        private void OnEnable()
        {
            _amplifiedEntitySpawnAudio = serializedObject.FindProperty("_amplifiedEntitySpawnAudio");

            _amplificationFactor = serializedObject.FindProperty("_amplificationFactor");
            _amplificationFactorRandomDeviation = serializedObject.FindProperty("_amplificationFactorRandomDeviation");

            _amplificationProbability = serializedObject.FindProperty("_amplificationProbability");
            _amplificationProbabilityRandomDeviation = serializedObject.FindProperty("_amplificationProbabilityRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_amplifiedEntitySpawnAudio, new GUIContent("Amplified entity spawn audio"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_amplificationFactor,
                                   AmplificationConfig.MinAplificationFactor,
                                   AmplificationConfig.MaxAmplificationFactor,
                                   "Amplification factor");
            EditorGUILayout.Slider(_amplificationFactorRandomDeviation,
                                   0f,
                                   _amplificationFactor.floatValue,
                                   "Max random deviation");

            _amplificationFactorRandomDeviation.floatValue = Mathf.Clamp(_amplificationFactorRandomDeviation.floatValue, 0f, _amplificationFactor.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_amplificationProbability,
                                   AmplificationConfig.MinAmplificationProbabiltiy,
                                   AmplificationConfig.MaxAmplificationProbability,
                                   "Amplification probability");
            EditorGUILayout.Slider(_amplificationProbabilityRandomDeviation,
                                   0f,
                                   _amplificationProbability.floatValue,
                                   "Max random deviation");

            _amplificationProbabilityRandomDeviation.floatValue = Mathf.Clamp(_amplificationProbabilityRandomDeviation.floatValue, 0f, _amplificationProbability.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as AmplificationConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}