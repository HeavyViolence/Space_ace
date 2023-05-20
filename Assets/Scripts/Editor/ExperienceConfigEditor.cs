using SpaceAce.Gameplay.Experience;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ExperienceConfig))]
    public class ExperienceConfigEditor : Editor
    {
        private SerializedProperty _experienceDepletionEnabled;

        private SerializedProperty _experienceDepletionPeriod;
        private SerializedProperty _experienceDepletionCurve;

        private void OnEnable()
        {
            _experienceDepletionEnabled = serializedObject.FindProperty("_experienceDepletionEnabled");

            _experienceDepletionPeriod = serializedObject.FindProperty("_experienceDepletionPeriod");
            _experienceDepletionCurve = serializedObject.FindProperty("_experienceDepletionCurve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_experienceDepletionEnabled, new GUIContent("Enable experienced depletion"));

            if (_experienceDepletionEnabled.boolValue == true)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Slider(_experienceDepletionPeriod,
                                       ExperienceConfig.MinExperienceDepletionDuration,
                                       ExperienceConfig.MaxExperienceDepletionDuration,
                                       "Depletion duration");
                EditorGUILayout.PropertyField(_experienceDepletionCurve, new GUIContent("Depletion curve"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}