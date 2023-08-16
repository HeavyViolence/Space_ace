using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(SponsorshipConfig))]
    public sealed class SponsorshipConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _experienceToCreditsConversionRate;
        private SerializedProperty _experienceToCreditsConversionRateRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _experienceToCreditsConversionRate = serializedObject.FindProperty("_experienceToCreditsConversionRate");
            _experienceToCreditsConversionRateRandomDeviation = serializedObject.FindProperty("_experienceToCreditsConversionRateRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();

            EditorGUILayout.Slider(_experienceToCreditsConversionRate,
                                   Sponsorship.MinExperienceToCreditsConversionRate,
                                   Sponsorship.MaxExperienceToCreditsConversionRate,
                                   "Experience to credits conversion rate");

            EditorGUILayout.Slider(_experienceToCreditsConversionRateRandomDeviation,
                                   0f,
                                   _experienceToCreditsConversionRate.floatValue,
                                   "Random deviation");

            _experienceToCreditsConversionRateRandomDeviation.floatValue = Mathf.Clamp(_experienceToCreditsConversionRateRandomDeviation.floatValue,
                                                                                       0f,
                                                                                       _experienceToCreditsConversionRate.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as SponsorshipConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}