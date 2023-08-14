using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(MeteorRouterConfig))]
    public sealed class MeteorRouterConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _meteorSpawnSpeedup;
        private SerializedProperty _meteorSpawnSpeedupRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _meteorSpawnSpeedup = serializedObject.FindProperty("_meteorSpawnSpeedup");
            _meteorSpawnSpeedupRandomDeviation = serializedObject.FindProperty("_meteorSpawnSpeedupRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_meteorSpawnSpeedup, MeteorRouter.MinMeteorSpawnSpeedup, MeteorRouter.MaxMeteorSpawnSpeedup, "Meteor spawn speedup");
            EditorGUILayout.Slider(_meteorSpawnSpeedupRandomDeviation, 0f, _meteorSpawnSpeedup.floatValue, "Random deviation");

            _meteorSpawnSpeedupRandomDeviation.floatValue = Mathf.Clamp(_meteorSpawnSpeedupRandomDeviation.floatValue,
                                                                        0f,
                                                                        _meteorSpawnSpeedup.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as MeteorRouterConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}