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
            private SerializedProperty _spaceBackgroundWidthDelta;
            private SerializedProperty _spaceBackgroundMaterials;
            private SerializedProperty _dustfieldPrefab;

            private SerializedProperty _levelConfigs;

            private bool _showScreenFaderSettings = false;
            private SerializedProperty _fadingCurve;

            private void OnEnable()
            {
                _idGeneratorSeed = serializedObject.FindProperty("_idGeneratorSeed");

                _spaceBackgroundWidthDelta = serializedObject.FindProperty("_spaceBackgroundWidthDelta");
                _spaceBackgroundMaterials = serializedObject.FindProperty("_spaceBackgroundMaterials");
                _dustfieldPrefab = serializedObject.FindProperty("_dustfieldPrefab");

                _levelConfigs = serializedObject.FindProperty("_levelConfigs");

                _fadingCurve = serializedObject.FindProperty("_fadingCurve");
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
                    EditorGUILayout.Slider(_spaceBackgroundWidthDelta, GameConfigurator.MinWidthDelta, GameConfigurator.MaxWidthDelta, "Width delta");
                    EditorGUILayout.PropertyField(_spaceBackgroundMaterials, new GUIContent("Space background materials"));
                    EditorGUILayout.PropertyField(_dustfieldPrefab, new GUIContent("Dustfield prefab"));
                }

                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(_levelConfigs, new GUIContent("Level configs"));

                EditorGUILayout.Separator();
                _showScreenFaderSettings = EditorGUILayout.Foldout(_showScreenFaderSettings, "Screen fader settings");

                if (_showScreenFaderSettings)
                {
                    EditorGUILayout.PropertyField(_fadingCurve, new GUIContent("Fading curve"));
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}