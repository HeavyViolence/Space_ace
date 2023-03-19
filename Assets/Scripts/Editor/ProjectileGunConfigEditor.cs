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
        private SerializedProperty _projectileHitEffect;

        private SerializedProperty _projectileSpeed;
        private SerializedProperty _projectileSpeedRandomDeviation;

        private SerializedProperty _projectileDamage;
        private SerializedProperty _projectileDamageRandomDeviation;

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

        private SerializedProperty _cameraShakeOnShot;

        private ProjectileGunConfig _target;

        private void OnEnable()
        {
            _gunGroupID = serializedObject.FindProperty("_gunGroupID");

            _projectile = serializedObject.FindProperty("_projectile");
            _projectileHitEffect = serializedObject.FindProperty("_projectileHitEffect");

            _projectileSpeed = serializedObject.FindProperty("_projectileSpeed");
            _projectileSpeedRandomDeviation = serializedObject.FindProperty("_projectileSpeedRandomDeviation");

            _projectileDamage = serializedObject.FindProperty("_projectileDamage");
            _projectileDamageRandomDeviation = serializedObject.FindProperty("_projectileDamageRandomDeviation");

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

            _cameraShakeOnShot = serializedObject.FindProperty("_cameraShakeOnShot");

            _target = (ProjectileGunConfig)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.IntSlider(_gunGroupID, ProjectileGunConfig.MinGunGroupID, ProjectileGunConfig.MaxGunGroupID, "Gun group ID");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_projectile, new GUIContent("Projectile"));
            EditorGUILayout.PropertyField(_projectileHitEffect, new GUIContent("Projectile hit effect"));
            EditorGUILayout.PropertyField(_fireAudio, new GUIContent("Fire audio"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_projectileSpeed, ProjectileGunConfig.MinProjectileSpeed, ProjectileGunConfig.MaxProjectileSpeed, "Projectile speed");
            EditorGUILayout.Slider(_projectileSpeedRandomDeviation, 0f, _projectileSpeed.floatValue, "Max random deviation");

            _projectileSpeedRandomDeviation.floatValue = Mathf.Clamp(_projectileSpeedRandomDeviation.floatValue, 0f, _projectileSpeed.floatValue);

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_projectileDamage, ProjectileGunConfig.MinProjectileDamage, ProjectileGunConfig.MaxProjectileDamage, "Projectile damage");
            EditorGUILayout.Slider(_projectileDamageRandomDeviation, 0f, _projectileDamage.floatValue, "Max random deviation");

            _projectileDamageRandomDeviation.floatValue = Mathf.Clamp(_projectileDamageRandomDeviation.floatValue, 0f, _projectileDamage.floatValue);

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
                _target.ApplySettings();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}