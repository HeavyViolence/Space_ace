using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(SpaceDebrisRouterConfig))]
    public sealed class SpaceDebrisRouterConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _spaceDebrisSpawnSpeedup;
        private SerializedProperty _spaceDebrisSpawnSpeedupRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _spaceDebrisSpawnSpeedup = serializedObject.FindProperty("_spaceDebrisSpawnSpeedup");
            _spaceDebrisSpawnSpeedupRandomDeviation = serializedObject.FindProperty("_spaceDebrisSpawnSpeedupRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_spaceDebrisSpawnSpeedup,
                                   SpaceDebrisRouter.MinSpaceDebrisSpawnSpeedup,
                                   SpaceDebrisRouter.MaxSpaceDebrisSpawnSpeedup,
                                   "Space debris spawn speedup");
            EditorGUILayout.Slider(_spaceDebrisSpawnSpeedupRandomDeviation,
                                   0f,
                                   _spaceDebrisSpawnSpeedup.floatValue,
                                   "Random deviation");

            _spaceDebrisSpawnSpeedupRandomDeviation.floatValue = Mathf.Clamp(_spaceDebrisSpawnSpeedupRandomDeviation.floatValue,
                                                                             0f,
                                                                             _spaceDebrisSpawnSpeedup.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as SpaceDebrisRouterConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}