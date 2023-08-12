using UnityEngine;
using UnityEditor;
using SpaceAce.Gameplay.Inventories;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(AtomizerConfig))]
    public sealed class AtomizerConfigEditor : InventoryItemConfigEditor
    {
        private SerializedProperty _entitiesToBeDestroyed;
        private SerializedProperty _entitiesToBeDestroyedRandomDeviation;

        protected override void OnEnable()
        {
            base.OnEnable();

            _entitiesToBeDestroyed = serializedObject.FindProperty("_entitiesToBeDestroyed");
            _entitiesToBeDestroyedRandomDeviation = serializedObject.FindProperty("_entitiesToBeDestroyedRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_entitiesToBeDestroyed,
                                      Atomizer.MinEntitiesToBeDestroyed,
                                      Atomizer.MaxEntitiesToBeDestroyed,
                                      "Entities to be destroyed");
            EditorGUILayout.IntSlider(_entitiesToBeDestroyedRandomDeviation,
                                      0,
                                      _entitiesToBeDestroyed.intValue,
                                      "Random deviation");

            _entitiesToBeDestroyedRandomDeviation.intValue = Mathf.Clamp(_entitiesToBeDestroyedRandomDeviation.intValue,
                                                                               0,
                                                                               _entitiesToBeDestroyed.intValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as AtomizerConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}