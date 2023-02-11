using SpaceAce.Architecture;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(GameConfigurator))]
    public sealed class GameConfiguratorEditor : Editor
    {
        private SerializedProperty _idGeneratorSeed;

        private bool _showSpaceBackgroundSettings = false;
        private SerializedProperty _spaceBackgroundWidthDelta;
        private SerializedProperty _mainMenuSpaceBackground;
        private SerializedProperty _spaceBackgrounds;
        private SerializedProperty _dustfieldPrefab;

        private SerializedProperty _levelConfigs;

        private bool _showScreenFaderSettings = false;
        private SerializedProperty _fadingCurve;

        private bool _showAudioSettings = false;
        private SerializedProperty _audioMixer;
        private SerializedProperty _music;

        private void OnEnable()
        {
            _idGeneratorSeed = serializedObject.FindProperty("_idGeneratorSeed");

            _spaceBackgroundWidthDelta = serializedObject.FindProperty("_spaceBackgroundWidthDelta");
            _mainMenuSpaceBackground = serializedObject.FindProperty("_mainMenuSpaceBackground");
            _spaceBackgrounds = serializedObject.FindProperty("_spaceBackgrounds");
            _dustfieldPrefab = serializedObject.FindProperty("_dustfieldPrefab");

            _levelConfigs = serializedObject.FindProperty("_levelConfigs");

            _fadingCurve = serializedObject.FindProperty("_fadingCurve");

            _audioMixer = serializedObject.FindProperty("_audioMixer");
            _music = serializedObject.FindProperty("_music");
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
                EditorGUILayout.PropertyField(_mainMenuSpaceBackground, new GUIContent("Main menu space background"));
                EditorGUILayout.PropertyField(_spaceBackgrounds, new GUIContent("Space backgrounds"));
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

            EditorGUILayout.Separator();
            _showAudioSettings = EditorGUILayout.Foldout(_showAudioSettings, "Audio settings");

            if (_showAudioSettings)
            {
                EditorGUILayout.PropertyField(_audioMixer, new GUIContent("Audio mixer"));
                EditorGUILayout.PropertyField(_music, new GUIContent("Music"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}