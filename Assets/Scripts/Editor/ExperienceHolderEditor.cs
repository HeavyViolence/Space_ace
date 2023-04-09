using SpaceAce.Gameplay.Experience;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ExperienceHolder))]
    public sealed class ExperienceHolderEditor : Editor
    {
        private SerializedProperty _config;

        private SerializedProperty _experienceDisabled;

        private void OnEnable()
        {
            _config = serializedObject.FindProperty("_config");

            _experienceDisabled = serializedObject.FindProperty("_experienceDisabled");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_experienceDisabled.boolValue == false)
            {
                EditorGUILayout.PropertyField(_config, new GUIContent("Config"));
            }

            EditorGUILayout.PropertyField(_experienceDisabled, new GUIContent("Disable experience"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}