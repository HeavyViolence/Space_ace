using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(AdvancedLearningConfig))]
    public sealed class AdvancedLearningConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _experienceBoost;
        private SerializedProperty _experienceBoostRandomDeviation;

        private SerializedProperty _experienceDepletionSlowdown;
        private SerializedProperty _experienceDepletionSlowdownRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _experienceBoost = serializedObject.FindProperty("_experienceBoost");
            _experienceBoostRandomDeviation = serializedObject.FindProperty("_experienceBoostRandomDeviation");

            _experienceDepletionSlowdown = serializedObject.FindProperty("_experienceDepletionSlowdown");
            _experienceDepletionSlowdownRandomDeviation = serializedObject.FindProperty("_experienceDepletionSlowdownRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_experienceBoost, AdvancedLearning.MinExperienceBoost, AdvancedLearning.MaxExperienceBoost, "Experience boost");
            EditorGUILayout.Slider(_experienceBoostRandomDeviation, 0f, _experienceBoost.floatValue, "Random deviation");

            _experienceBoostRandomDeviation.floatValue = Mathf.Clamp(_experienceBoostRandomDeviation.floatValue, 0f, _experienceBoost.floatValue);

            EditorGUILayout.Separator();

            EditorGUILayout.Slider(_experienceDepletionSlowdown,
                                   AdvancedLearning.MinExperienceDepletionSlowdown,
                                   AdvancedLearning.MaxExperienceDepletionSlowdown,
                                   "Experience depletion slowdown");

            EditorGUILayout.Slider(_experienceDepletionSlowdownRandomDeviation, 0f, _experienceDepletionSlowdown.floatValue, "Random deviation");

            _experienceDepletionSlowdownRandomDeviation.floatValue = Mathf.Clamp(_experienceDepletionSlowdownRandomDeviation.floatValue,
                                                                                 0f,
                                                                                 _experienceDepletionSlowdown.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as AdvancedLearningConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}