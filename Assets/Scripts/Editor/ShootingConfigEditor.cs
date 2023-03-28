using SpaceAce.Gameplay.Shooting;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ShootingConfig))]
    public sealed class ShootingConfigEditor : Editor
    {
        private SerializedProperty _firstFireDelay;
        private SerializedProperty _firstFireDelayRandomDeviation;

        private SerializedProperty _nextFireDelay;
        private SerializedProperty _nextFireDelayRandomDeviation;

        private SerializedProperty _enableWeaponsSwitch;

        private SerializedProperty _firstWeaponsSwitchDelay;
        private SerializedProperty _firstWeaponsSwitchDelayRandomDeviation;

        private SerializedProperty _nextWeaponsSwitchDelay;
        private SerializedProperty _nextWeaponsSwitchDelayRandomDeviation;

        private ShootingConfig _target;

        private void OnEnable()
        {
            _firstFireDelay = serializedObject.FindProperty("_firstFireDelay");
            _firstFireDelayRandomDeviation = serializedObject.FindProperty("_firstFireDelayRandomDeviation");

            _nextFireDelay = serializedObject.FindProperty("_nextFireDelay");
            _nextFireDelayRandomDeviation = serializedObject.FindProperty("_nextFireDelayRandomDeviation");

            _enableWeaponsSwitch = serializedObject.FindProperty("_enableWeaponsSwitch");

            _firstWeaponsSwitchDelay = serializedObject.FindProperty("_firstWeaponsSwitchDelay");
            _firstWeaponsSwitchDelayRandomDeviation = serializedObject.FindProperty("_firstWeaponsSwitchDelayRandomDeviation");

            _nextWeaponsSwitchDelay = serializedObject.FindProperty("_nextWeaponsSwitchDelay");
            _nextWeaponsSwitchDelayRandomDeviation = serializedObject.FindProperty("_nextWeaponsSwitchDelayRandomDeviation");

            _target = (ShootingConfig)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Slider(_firstFireDelay, ShootingConfig.MinFirstFireDelay, ShootingConfig.MaxFirstFireDelay, "First fire delay");
            EditorGUILayout.Slider(_firstFireDelayRandomDeviation, 0f, _firstFireDelay.floatValue, "Max random deviation");

            _firstFireDelayRandomDeviation.floatValue = Mathf.Clamp(_firstFireDelayRandomDeviation.floatValue, 0f, _firstFireDelay.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_nextFireDelay, ShootingConfig.MinNextFireDelay, ShootingConfig.MaxNextFireDelay, "Next fire delay");
            EditorGUILayout.Slider(_nextFireDelayRandomDeviation, 0f, _nextFireDelay.floatValue, "Max random deviation");

            _nextFireDelayRandomDeviation.floatValue = Mathf.Clamp(_nextFireDelayRandomDeviation.floatValue, 0f, _nextFireDelay.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_enableWeaponsSwitch, new GUIContent("Enable weapons switch"));

            if (_enableWeaponsSwitch.boolValue == true)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Slider(_firstWeaponsSwitchDelay,
                                       ShootingConfig.MinFirstWeaponsSwitchDelay,
                                       ShootingConfig.MaxFirstWeaponsSwitchDelay,
                                       "First weapons switch delay");
                EditorGUILayout.Slider(_firstWeaponsSwitchDelayRandomDeviation,
                                       0f,
                                       _firstWeaponsSwitchDelay.floatValue,
                                       "Max random deviation");

                _firstWeaponsSwitchDelayRandomDeviation.floatValue = Mathf.Clamp(_firstWeaponsSwitchDelayRandomDeviation.floatValue,
                                                                                 0f,
                                                                                 _firstWeaponsSwitchDelay.floatValue);

                EditorGUILayout.Separator();
                EditorGUILayout.Slider(_nextWeaponsSwitchDelay,
                                       ShootingConfig.MinNextWeaponsSwitchDelay,
                                       ShootingConfig.MaxNextWeaponsSwitchDelay,
                                       "Next weapons switch delay");
                EditorGUILayout.Slider(_nextWeaponsSwitchDelayRandomDeviation,
                                       0f,
                                       _nextWeaponsSwitchDelay.floatValue,
                                       "Max random deviation");

                _nextWeaponsSwitchDelayRandomDeviation.floatValue = Mathf.Clamp(_nextWeaponsSwitchDelayRandomDeviation.floatValue,
                                                                                0f,
                                                                                _nextWeaponsSwitchDelay.floatValue);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                _target.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}