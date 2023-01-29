using SpaceAce.Architecture;
using UnityEditor;
using UnityEngine;

namespace SpaceAce
{
    namespace Editors
    {
        [CustomEditor(typeof(GameConfigurator))]
        public sealed class GameConfiguratorEditor : Editor
        {
            private SerializedProperty _idGeneratorSeed;

            private bool _showSpaceBackgroundSettings = false;
            private SerializedProperty _spaceBackgroundPrefab;
            private SerializedProperty _spaceBackgroundMaterials;

            private void OnEnable()
            {
                _idGeneratorSeed = serializedObject.FindProperty("_idGeneratorSeed");

                _spaceBackgroundPrefab = serializedObject.FindProperty("_spaceBackgroundPrefab");
                _spaceBackgroundMaterials = serializedObject.FindProperty("_spaceBackgroundMaterials");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.LabelField($"ID generator seed: {_idGeneratorSeed.intValue}");

                if (_idGeneratorSeed.intValue == 0)
                {
                    _idGeneratorSeed.intValue = Random.Range(1, int.MaxValue);
                }

                EditorGUILayout.Separator();
                _showSpaceBackgroundSettings = EditorGUILayout.Foldout(_showSpaceBackgroundSettings, "Space background settings");

                if (_showSpaceBackgroundSettings)
                {
                    EditorGUILayout.PropertyField(_spaceBackgroundPrefab, new GUIContent("Space background prefab"));
                    EditorGUILayout.PropertyField(_spaceBackgroundMaterials, new GUIContent("Space background materials"));
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}