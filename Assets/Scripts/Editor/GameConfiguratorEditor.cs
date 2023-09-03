using SpaceAce.Architecture;
using SpaceAce.Main;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(GameConfigurator))]
    public sealed class GameConfiguratorEditor : Editor
    {
        private bool _showCameraSettings = false;
        private SerializedProperty _cameraSize;

        private bool _showSpaceBackgroundSettings = false;
        private SerializedProperty _spaceBackgroundWidthDelta;
        private SerializedProperty _mainMenuSpaceBackground;
        private SerializedProperty _spaceBackgrounds;
        private SerializedProperty _dustfieldPrefab;

        private SerializedProperty _levelConfigs;

        private bool _showScreenFaderSettings = false;
        private SerializedProperty _fadingCurve;
        private SerializedProperty _fadingColor;

        private bool _showAudioSettings = false;
        private SerializedProperty _audioMixer;
        private SerializedProperty _music;
        private SerializedProperty _bossSpawnAlarm;

        private bool _showUISettings = false;
        private SerializedProperty _uiAssets;

        private bool _showPlayerSettings = false;
        private SerializedProperty _defaultPlayerShip;
        private SerializedProperty _objectPoolEntryLookupTable;

        private bool _showEntityVisualizationSettings = false;
        private SerializedProperty _itemIconsConfig;
        private SerializedProperty _itemRarityColorsConfig;
        private SerializedProperty _lootItemBox;

        private bool _showOtherSettings = false;
        private SerializedProperty _easingCurveIn;
        private SerializedProperty _easingCurveOut;
        private SerializedProperty _easingCurveInOut;

        private void OnEnable()
        {
            _cameraSize = serializedObject.FindProperty("_cameraSize");

            _spaceBackgroundWidthDelta = serializedObject.FindProperty("_spaceBackgroundWidthDelta");
            _mainMenuSpaceBackground = serializedObject.FindProperty("_mainMenuSpaceBackground");
            _spaceBackgrounds = serializedObject.FindProperty("_spaceBackgrounds");
            _dustfieldPrefab = serializedObject.FindProperty("_dustfieldPrefab");

            _levelConfigs = serializedObject.FindProperty("_levelConfigs");

            _fadingCurve = serializedObject.FindProperty("_fadingCurve");
            _fadingColor = serializedObject.FindProperty("_fadingColor");

            _audioMixer = serializedObject.FindProperty("_audioMixer");
            _music = serializedObject.FindProperty("_music");
            _bossSpawnAlarm = serializedObject.FindProperty("_bossSpawnAlarm");

            _uiAssets = serializedObject.FindProperty("_uiAssets");

            _defaultPlayerShip = serializedObject.FindProperty("_defaultPlayerShip");
            _objectPoolEntryLookupTable = serializedObject.FindProperty("_objectPoolEntryLookupTable");

            _itemIconsConfig = serializedObject.FindProperty("_itemIconsConfig");
            _itemRarityColorsConfig = serializedObject.FindProperty("_itemRarityColorsConfig");
            _lootItemBox = serializedObject.FindProperty("_lootItemBox");

            _easingCurveIn = serializedObject.FindProperty("_easingCurveIn");
            _easingCurveOut = serializedObject.FindProperty("_easingCurveOut");
            _easingCurveInOut = serializedObject.FindProperty("_easingCurveInOut");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _showCameraSettings = EditorGUILayout.Foldout(_showCameraSettings, "Camera");

            if (_showCameraSettings == true)
            {
                EditorGUILayout.Slider(_cameraSize, MasterCameraHolder.MinCameraSize, MasterCameraHolder.MaxCameraSize, "Camera size");
            }

            EditorGUILayout.Separator();
            _showSpaceBackgroundSettings = EditorGUILayout.Foldout(_showSpaceBackgroundSettings, "Space background");

            if (_showSpaceBackgroundSettings == true)
            {
                EditorGUILayout.Slider(_spaceBackgroundWidthDelta, GameConfigurator.MinWidthDelta, GameConfigurator.MaxWidthDelta, "Width delta");
                EditorGUILayout.PropertyField(_mainMenuSpaceBackground, new GUIContent("Main menu space background"));
                EditorGUILayout.PropertyField(_spaceBackgrounds, new GUIContent("Space backgrounds"));
                EditorGUILayout.PropertyField(_dustfieldPrefab, new GUIContent("Dustfield prefab"));
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_levelConfigs, new GUIContent("Level configs"));

            EditorGUILayout.Separator();
            _showScreenFaderSettings = EditorGUILayout.Foldout(_showScreenFaderSettings, "Screen fader");

            if (_showScreenFaderSettings == true)
            {
                EditorGUILayout.PropertyField(_fadingCurve, new GUIContent("Fading curve"));
                EditorGUILayout.PropertyField(_fadingColor, new GUIContent("Fading color"));
            }

            EditorGUILayout.Separator();
            _showAudioSettings = EditorGUILayout.Foldout(_showAudioSettings, "Audio");

            if (_showAudioSettings == true)
            {
                EditorGUILayout.PropertyField(_audioMixer, new GUIContent("Audio mixer"));
                EditorGUILayout.PropertyField(_music, new GUIContent("Music"));
                EditorGUILayout.PropertyField(_bossSpawnAlarm, new GUIContent("Boss spawn alarm"));
            }

            EditorGUILayout.Separator();
            _showUISettings = EditorGUILayout.Foldout(_showUISettings, "UI settings");

            if (_showUISettings == true)
            {
                EditorGUILayout.PropertyField(_uiAssets, new GUIContent("UI assets"));
            }

            EditorGUILayout.Separator();
            _showPlayerSettings = EditorGUILayout.Foldout(_showPlayerSettings, "Player");

            if (_showPlayerSettings == true)
            {
                EditorGUILayout.PropertyField(_defaultPlayerShip, new GUIContent("Default player ship"));
                EditorGUILayout.PropertyField(_objectPoolEntryLookupTable, new GUIContent("Object pool entry lookup table"));
            }

            EditorGUILayout.Separator();
            _showEntityVisualizationSettings = EditorGUILayout.Foldout(_showEntityVisualizationSettings, "Entity visualization");

            if (_showEntityVisualizationSettings == true)
            {
                EditorGUILayout.PropertyField(_itemIconsConfig, new GUIContent("Item icons"));
                EditorGUILayout.PropertyField(_itemRarityColorsConfig, new GUIContent("Item rarity colors"));
                EditorGUILayout.PropertyField(_lootItemBox, new GUIContent("Loot item box"));
            }

            EditorGUILayout.Separator();
            _showOtherSettings = EditorGUILayout.Foldout(_showOtherSettings, "Other");

            if (_showOtherSettings  == true)
            {
                EditorGUILayout.PropertyField(_easingCurveIn, new GUIContent("Easing curve in"));
                EditorGUILayout.PropertyField(_easingCurveOut, new GUIContent("Easing curve out"));
                EditorGUILayout.PropertyField(_easingCurveInOut, new GUIContent("Easing curve in and out"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}