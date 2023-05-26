using SpaceAce.Gameplay.Shooting;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(BombConfig))]
    public sealed class BombConfigEditor : Editor
    {
        private SerializedProperty _damage;
        private SerializedProperty _damageRandomDeviation;

        private SerializedProperty _damageDelay;
        private SerializedProperty _damageDelayRandomDeviation;

        private void OnEnable()
        {
            _damage = serializedObject.FindProperty("_damage");
            _damageRandomDeviation = serializedObject.FindProperty("_damageRandomDeviation");

            _damageDelay = serializedObject.FindProperty("_damageDelay");
            _damageDelayRandomDeviation = serializedObject.FindProperty("_damageDelayRandomDeviation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Slider(_damage, 0f, BombConfig.MaxDamage, "Damage");
            EditorGUILayout.Slider(_damageRandomDeviation, 0f, _damage.floatValue, "Max random deviation");

            _damageRandomDeviation.floatValue = Mathf.Clamp(_damageRandomDeviation.floatValue, 0f, _damage.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_damageDelay, 0f, BombConfig.MaxDamageDelay, "Damage delay");
            EditorGUILayout.Slider(_damageDelayRandomDeviation, 0f, _damageDelay.floatValue, "Max random deviation");

            _damageDelayRandomDeviation.floatValue = Mathf.Clamp(_damageDelayRandomDeviation.floatValue, 0f, _damageDelay.floatValue);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply setings"))
            {
                var config = target as BombConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}