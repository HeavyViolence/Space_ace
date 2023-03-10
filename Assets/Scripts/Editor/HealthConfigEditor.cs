using SpaceAce.Gameplay.Damageables;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(HealthConfig))]
    public sealed class HealthConfigEditor : Editor
    {
        private SerializedProperty _healthLimit;
        private SerializedProperty _healthLimitRandomDeviation;

        private SerializedProperty _regenEnabled;
        private SerializedProperty _regenPerSecond;
        private SerializedProperty _regenPerSecondRandomDeviation;

        private SerializedProperty _deathEffect;
        private SerializedProperty _deathAudio;

        private SerializedProperty _cameraShakeOnDamagedEnabled;

        private void OnEnable()
        {
            _healthLimit = serializedObject.FindProperty("_healthLimit");
            _healthLimitRandomDeviation = serializedObject.FindProperty("_healthLimitRandomDeviation");

            _regenEnabled = serializedObject.FindProperty("_regenEnabled");
            _regenPerSecond = serializedObject.FindProperty("_regenPerSecond");
            _regenPerSecondRandomDeviation = serializedObject.FindProperty("_regenPerSecondRandomDeviation");

            _deathEffect = serializedObject.FindProperty("_deathEffect");
            _deathAudio = serializedObject.FindProperty("_deathAudio");

            _cameraShakeOnDamagedEnabled = serializedObject.FindProperty("_cameraShakeOnDamagedEnabled");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Slider(_healthLimit, HealthConfig.MinHealth, HealthConfig.MaxHealth, "Max health");
            EditorGUILayout.Slider(_healthLimitRandomDeviation, 0f, _healthLimit.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_regenEnabled, new GUIContent("Enable health regeneration"));

            if (_regenEnabled.boolValue == true)
            {
                EditorGUILayout.Slider(_regenPerSecond, HealthConfig.MinHealthRegenPerSecond, HealthConfig.MaxHealthRegenPerSecond, "Regen per second");
                EditorGUILayout.Slider(_regenPerSecondRandomDeviation, 0f, _regenPerSecond.floatValue, "Max random deviation");
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_deathEffect, new GUIContent("Death effect"));
            EditorGUILayout.PropertyField(_deathAudio, new GUIContent("Death audio"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_cameraShakeOnDamagedEnabled, new GUIContent("Camera shake upon damage"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}