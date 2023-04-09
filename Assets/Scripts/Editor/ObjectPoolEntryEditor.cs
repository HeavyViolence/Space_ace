using SpaceAce.Main.ObjectPooling;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ObjectPoolEntry))]
    public sealed class ObjectPoolEntryEditor : Editor
    {
        private SerializedProperty _prefab;
        private SerializedProperty _anchorName;

        private void OnEnable()
        {
            _prefab = serializedObject.FindProperty("_prefab");
            _anchorName = serializedObject.FindProperty("_anchorName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_prefab, new GUIContent("Prefab"));
            EditorGUILayout.PropertyField(_anchorName, new GUIContent("Anchor name"));

            EditorGUILayout.Separator();

            if (GUILayout.Button("Set proper anchor name"))
            {
                var entry = target as ObjectPoolEntry;
                _anchorName.stringValue = entry.Prefab.name.ToLower();
            }

            if (GUILayout.Button("Clear anchor name"))
            {
                if (string.IsNullOrEmpty(_anchorName.stringValue) == false &&
                    string.IsNullOrWhiteSpace(_anchorName.stringValue) == false)
                {
                    _anchorName.stringValue = string.Empty;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}