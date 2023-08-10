using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(EMPConfig))]
    public sealed class EMPConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _jamProbability;
        private SerializedProperty _jamProbabilityRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _jamProbability = serializedObject.FindProperty("_jamProbability");
            _jamProbabilityRandomDeviation = serializedObject.FindProperty("_jamProbabilityRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_jamProbability, EMP.MinJamProbability, EMP.MaxJamProbability, "Jam probability");
            EditorGUILayout.Slider(_jamProbabilityRandomDeviation, 0f, _jamProbability.floatValue, "Random deviation");

            _jamProbabilityRandomDeviation.floatValue = Mathf.Clamp(_jamProbabilityRandomDeviation.floatValue, 0f, _jamProbability.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as EMPConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}