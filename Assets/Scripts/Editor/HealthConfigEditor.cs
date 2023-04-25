using SpaceAce.Gameplay.Damageables;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(HealthConfig))]
    public sealed class HealthConfigEditor : Editor
    {
        private SerializedProperty _maxHealth;
        private SerializedProperty _maxHealthRandomDeviation;

        private SerializedProperty _regenerationEnabled;
        private SerializedProperty _regenerationPerSecond;
        private SerializedProperty _regenerationPerSecondRandomDeviation;

        private SerializedProperty _deathEffect;
        private SerializedProperty _deathAudio;

        private SerializedProperty _cameraShakeOnDamaged;

        private void OnEnable()
        {
            _maxHealth = serializedObject.FindProperty("_maxHealth");
            _maxHealthRandomDeviation = serializedObject.FindProperty("_maxHealthRandomDeviation");

            _regenerationEnabled = serializedObject.FindProperty("_regenerationEnabled");
            _regenerationPerSecond = serializedObject.FindProperty("_regenerationPerSecond");
            _regenerationPerSecondRandomDeviation = serializedObject.FindProperty("_regenerationPerSecondRandomDeviation");

            _deathEffect = serializedObject.FindProperty("_deathEffect");
            _deathAudio = serializedObject.FindProperty("_deathAudio");

            _cameraShakeOnDamaged = serializedObject.FindProperty("_cameraShakeOnDamaged");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Slider(_maxHealth, HealthConfig.MinHealth, HealthConfig.MaxHealth, "Max health");
            EditorGUILayout.Slider(_maxHealthRandomDeviation, 0f, _maxHealth.floatValue, "Max random deviation");

            _maxHealthRandomDeviation.floatValue = Mathf.Clamp(_maxHealthRandomDeviation.floatValue, 0f, _maxHealth.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_regenerationEnabled, new GUIContent("Enable health regeneration"));

            if (_regenerationEnabled.boolValue == true)
            {
                EditorGUILayout.Slider(_regenerationPerSecond,
                                       HealthConfig.MinHealthRegenerationPerSecond,
                                       HealthConfig.MaxHealthRegenerationPerSecond,
                                       "Regeneration per second");

                EditorGUILayout.Slider(_regenerationPerSecondRandomDeviation,
                                       0f,
                                       _regenerationPerSecond.floatValue,
                                       "Max random deviation");

                _regenerationPerSecondRandomDeviation.floatValue = Mathf.Clamp(_regenerationPerSecondRandomDeviation.floatValue, 0f, _regenerationPerSecond.floatValue);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_deathEffect, new GUIContent("Death effect"));
            EditorGUILayout.PropertyField(_deathAudio, new GUIContent("Death audio"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_cameraShakeOnDamaged, new GUIContent("Camera shake on damage"));

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply settings"))
            {
                var config = target as HealthConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}