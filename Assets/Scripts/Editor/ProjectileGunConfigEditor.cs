using SpaceAce.Gameplay.Shooting;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(ProjectileGunConfig))]
    public sealed class ProjectileGunConfigEditor : GunConfigEditor
    {
        private SerializedProperty _projectile;
        private SerializedProperty _hitEffect;

        private SerializedProperty _damage;
        private SerializedProperty _damageRandomDeviation;

        private SerializedProperty _projectileSpeed;
        private SerializedProperty _projectileSpeedRandomDeviation;

        private SerializedProperty _fireRate;
        private SerializedProperty _fireRateRandomDeviation;

        private SerializedProperty _projectilesPerShot;
        private SerializedProperty _projectilesPerShotRandomDeviation;

        private SerializedProperty _dispersion;
        private SerializedProperty _convergenceAngle;

        private SerializedProperty _cameraShakeOnShotFiredEnabled;

        protected override void OnEnable()
        {
            base.OnEnable();

            _projectile = serializedObject.FindProperty("_projectile");
            _hitEffect = serializedObject.FindProperty("_hitEffect");

            _damage = serializedObject.FindProperty("_damage");
            _damageRandomDeviation = serializedObject.FindProperty("_damageRandomDeviation");

            _projectileSpeed = serializedObject.FindProperty("_projectileSpeed");
            _projectileSpeedRandomDeviation = serializedObject.FindProperty("_projectileSpeedRandomDeviation");

            _fireRate = serializedObject.FindProperty("_fireRate");
            _fireRateRandomDeviation = serializedObject.FindProperty("_fireRateRandomDeviation");

            _projectilesPerShot = serializedObject.FindProperty("_projectilesPerShot");
            _projectilesPerShotRandomDeviation = serializedObject.FindProperty("_projectilesPerShotRandomDeviation");

            _dispersion = serializedObject.FindProperty("_dispersion");
            _convergenceAngle = serializedObject.FindProperty("_convergenceAngle");

            _cameraShakeOnShotFiredEnabled = serializedObject.FindProperty("_cameraShakeOnShotFiredEnabled");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_projectile, new GUIContent("Projectile"));
            EditorGUILayout.PropertyField(_hitEffect, new GUIContent("Hit effect"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_damage,
                                   ProjectileGunConfig.MinProjectileDamage,
                                   ProjectileGunConfig.MaxProjectileDamage,
                                   "Projectile damage");
            EditorGUILayout.Slider(_damageRandomDeviation, 0f, _damage.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_projectileSpeed,
                                   ProjectileGunConfig.MinProjectileSpeed,
                                   ProjectileGunConfig.MaxProjectileSpeed,
                                   "Projectile speed");
            EditorGUILayout.Slider(_projectileSpeedRandomDeviation, 0f, _projectileSpeed.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_fireRate,
                                   ProjectileGunConfig.MinFireRate,
                                   ProjectileGunConfig.MaxFirerate,
                                   "Fire rate");
            EditorGUILayout.Slider(_fireRateRandomDeviation, 0f, _fireRate.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.IntSlider(_projectilesPerShot,
                                      ProjectileGunConfig.MinProjectilesPerShot,
                                      ProjectileGunConfig.MaxProjectilesPerShot,
                                      "Projectiles per shot");
            EditorGUILayout.IntSlider(_projectilesPerShotRandomDeviation, 0, _projectilesPerShot.intValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_dispersion,
                                   ProjectileGunConfig.MinDispersion,
                                   ProjectileGunConfig.MaxDispersion,
                                   "Dispersion");
            EditorGUILayout.Slider(_convergenceAngle,
                                   ProjectileGunConfig.MinConvergenceAngle,
                                   ProjectileGunConfig.MaxConvergenceAngle,
                                   "Convergence angle");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_cameraShakeOnShotFiredEnabled, new GUIContent("Camera shake on shot"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}