using SpaceAce.Gameplay.Shooting;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ProjectileGunConfig))]
    public sealed class ProjectileGunConfigEditor : Editor
    {
        private SerializedProperty _gunGroupID;

        private SerializedProperty _projectile;
        private SerializedProperty _hitEffect;

        private SerializedProperty _behaviour;
        private SerializedProperty _targetSupplier;

        private SerializedProperty _speed;
        private SerializedProperty _speedRandomDeviation;

        private SerializedProperty _rotationConfig;

        private SerializedProperty _damage;
        private SerializedProperty _damageRandomDeviation;

        private SerializedProperty _projectilesPerShot;
        private SerializedProperty _projectilesPerShotRandomDeviation;

        private SerializedProperty _fireDuration;
        private SerializedProperty _fireDurationRandomDeviation;

        private SerializedProperty _fireRate;
        private SerializedProperty _fireRateRandomDeviation;

        private SerializedProperty _cooldown;
        private SerializedProperty _cooldownRandomDeviation;

        private SerializedProperty _dispersion;
        private SerializedProperty _convergenceAngle;

        private SerializedProperty _fireAudio;
        private SerializedProperty _hitAudio;

        private SerializedProperty _cameraShakeOnShot;

        private void OnEnable()
        {
            _gunGroupID = serializedObject.FindProperty("_gunGroupID");

            _projectile = serializedObject.FindProperty("_projectile");
            _hitEffect = serializedObject.FindProperty("_hitEffect");

            _behaviour = serializedObject.FindProperty("_behaviour");
            _targetSupplier = serializedObject.FindProperty("_targetSupplier");

            _speed = serializedObject.FindProperty("_speed");
            _speedRandomDeviation = serializedObject.FindProperty("_speedRandomDeviation");

            _rotationConfig = serializedObject.FindProperty("_rotationConfig");

            _damage = serializedObject.FindProperty("_damage");
            _damageRandomDeviation = serializedObject.FindProperty("_damageRandomDeviation");

            _projectilesPerShot = serializedObject.FindProperty("_projectilesPerShot");
            _projectilesPerShotRandomDeviation = serializedObject.FindProperty("_projectilesPerShotRandomDeviation");

            _fireDuration = serializedObject.FindProperty("_fireDuration");
            _fireDurationRandomDeviation = serializedObject.FindProperty("_fireDurationRandomDeviation");

            _fireRate = serializedObject.FindProperty("_fireRate");
            _fireRateRandomDeviation = serializedObject.FindProperty("_fireRateRandomDeviation");

            _cooldown = serializedObject.FindProperty("_cooldown");
            _cooldownRandomDeviation = serializedObject.FindProperty("_cooldownRandomDeviation");

            _dispersion = serializedObject.FindProperty("_dispersion");
            _convergenceAngle = serializedObject.FindProperty("_convergenceAngle");

            _fireAudio = serializedObject.FindProperty("_fireAudio");
            _hitAudio = serializedObject.FindProperty("_hitAudio");

            _cameraShakeOnShot = serializedObject.FindProperty("_cameraShakeOnShot");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.IntSlider(_gunGroupID, ProjectileGunConfig.MinGunGroupID, ProjectileGunConfig.MaxGunGroupID, "Gun group ID");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_projectile, new GUIContent("Projectile"));
            EditorGUILayout.PropertyField(_behaviour, new GUIContent("Movement behaviour"));
            EditorGUILayout.PropertyField(_targetSupplier, new GUIContent("Target supplier"));
            EditorGUILayout.PropertyField(_hitEffect, new GUIContent("Hit effect"));
            EditorGUILayout.PropertyField(_fireAudio, new GUIContent("Fire audio"));
            EditorGUILayout.PropertyField(_hitAudio, new GUIContent("Hit audio"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_speed, ProjectileGunConfig.MinSpeed, ProjectileGunConfig.MaxSpeed, "Speed");
            EditorGUILayout.Slider(_speedRandomDeviation, 0f, _speed.floatValue, "Max random deviation");

            _speedRandomDeviation.floatValue = Mathf.Clamp(_speedRandomDeviation.floatValue, 0f, _speed.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_rotationConfig, new GUIContent("Rotation config"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_damage, ProjectileGunConfig.MinDamage, ProjectileGunConfig.MaxDamage, "Damage");
            EditorGUILayout.Slider(_damageRandomDeviation, 0f, _damage.floatValue, "Max random deviation");

            _damageRandomDeviation.floatValue = Mathf.Clamp(_damageRandomDeviation.floatValue, 0f, _damage.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_projectilesPerShot, ProjectileGunConfig.MinProjectilesPerShot, ProjectileGunConfig.MaxProjectilesPerShot, "Projectiles per shot");
            EditorGUILayout.IntSlider(_projectilesPerShotRandomDeviation, 0, _projectilesPerShot.intValue, "Max random deviation");

            _projectilesPerShotRandomDeviation.intValue = Mathf.Clamp(_projectilesPerShotRandomDeviation.intValue, 0, _projectilesPerShot.intValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_fireDuration, ProjectileGunConfig.MinFireDuration, ProjectileGunConfig.MaxFireDuration, "Fire duration");
            EditorGUILayout.Slider(_fireDurationRandomDeviation, 0f, _fireDuration.floatValue, "Max random deviation");

            _fireDurationRandomDeviation.floatValue = Mathf.Clamp(_fireDurationRandomDeviation.floatValue, 0f, _fireDuration.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_fireRate, ProjectileGunConfig.MinFireRate, ProjectileGunConfig.MaxFireRate, "Fire rate");
            EditorGUILayout.Slider(_fireRateRandomDeviation, 0f, _fireRate.floatValue, "Max random deviation");

            _fireRateRandomDeviation.floatValue = Mathf.Clamp(_fireRateRandomDeviation.floatValue, 0f, _fireRate.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_cooldown, ProjectileGunConfig.MinCooldown, ProjectileGunConfig.MaxCooldown, "Cooldown");
            EditorGUILayout.Slider(_cooldownRandomDeviation, 0f, _cooldown.floatValue, "Max random deviation");

            _cooldownRandomDeviation.floatValue = Mathf.Clamp(_cooldownRandomDeviation.floatValue, 0f, _cooldown.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_dispersion, ProjectileGunConfig.MinDispersion, ProjectileGunConfig.MaxDispersion, "Fire dispersion");
            EditorGUILayout.Slider(_convergenceAngle, ProjectileGunConfig.MinConvergenceAngle, ProjectileGunConfig.MaxConvergenceAngle, "Gun convergence angle");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_cameraShakeOnShot, new GUIContent("Camera shake on shot"));

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Apply settings"))
            {
                var config = target as ProjectileGunConfig;
                config.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}