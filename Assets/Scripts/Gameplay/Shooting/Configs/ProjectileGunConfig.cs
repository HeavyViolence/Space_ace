using SpaceAce.Auxiliary;
using SpaceAce.Main.Audio;
using SpaceAce.Main.ObjectPooling;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Projectile gun config", menuName = "Space ace/Configs/Shooting/Projectile gun config")]
    public sealed class ProjectileGunConfig : ScriptableObject
    {
        public const int MinGunGroupID = 0;
        public const int MaxGunGroupID = 9;

        public const float MinProjectileSpeed = 10f;
        public const float MaxProjectileSpeed = 100f;

        public const float MinProjectileDamage = 10f;
        public const float MaxProjectileDamage = 1000f;

        public const int MinProjectilesPerShot = 1;
        public const int MaxProjectilesPerShot = 10;

        public const float MinFireDuration = 1f;
        public const float MaxFireDuration = 10f;

        public const float MinFireRate = 1f;
        public const float MaxFireRate = 20f;

        public const float MinCooldown = 1f;
        public const float MaxCooldown = 10f;

        public const float MinDispersion = 0f;
        public const float MaxDispersion = 0.1f;
        public const float DefaultDispersion = 0.05f;

        public const float MinConvergenceAngle = 0f;
        public const float MaxConvergenceAngle = 0.1f;
        public const float DefaultConvergenceAngle = 0.02f;

        [SerializeField] private int _gunGroupID = MinGunGroupID;

        [SerializeField] private ObjectPoolEntry _projectile;
        [SerializeField] private ObjectPoolEntry _projectileHitEffect;

        [SerializeField] private float _projectileSpeed = MinProjectileSpeed;
        [SerializeField] private float _projectileSpeedRandomDeviation = 0f;

        [SerializeField] private float _projectileDamage = MinProjectileDamage;
        [SerializeField] private float _projectileDamageRandomDeviation = 0f;

        [SerializeField] private int _projectilesPerShot = MinProjectilesPerShot;
        [SerializeField] private int _projectilesPerShotRandomDeviation = 0;

        [SerializeField] private float _fireDuration = MinFireDuration;
        [SerializeField] private float _fireDurationRandomDeviation = 0f;

        [SerializeField] private float _fireRate = MinFireRate;
        [SerializeField] private float _fireRateRandomDeviation = 0f;

        [SerializeField] private float _cooldown = MinCooldown;
        [SerializeField] private float _cooldownRandomDeviation = 0f;

        [SerializeField] private float _dispersion = DefaultDispersion;
        [SerializeField] private float _convergenceAngle = DefaultConvergenceAngle;

        [SerializeField] private AudioCollection _fireAudio;

        [SerializeField] private bool _cameraShakeOnShot = false;

        public int GunGroupID => _gunGroupID;

        public ObjectPoolEntry Projectile => _projectile;
        public ObjectPoolEntry ProjectileHitEffect => _projectileHitEffect;

        public RangedFloat ProjectileSpeed { get; private set; }
        public RangedFloat ProjectileDamage { get; private set; }
        public RangedInt ProjectilesPerShot { get; private set; }
        public RangedFloat FireDuration { get; private set; }
        public RangedFloat FireRate { get; private set; }
        public RangedFloat Cooldown { get; private set; }
        public RangedFloat Dispersion { get; private set; }

        public AudioCollection FireAudio => _fireAudio;

        public bool CameraShakeOnShotEnabled => _cameraShakeOnShot;

        public float GetConvergenceAngle(bool isRightHandedGun) => isRightHandedGun ? -1f * _convergenceAngle : _convergenceAngle;

        private void OnEnable()
        {
            ApplySettings();
        }

        public void EnsureNecessaryObjectPoolsExistence()
        {
            _projectile.EnsureObjectPoolExistence();
            _projectileHitEffect.EnsureObjectPoolExistence();
        }

        public void ApplySettings()
        {
            ProjectileSpeed = new(_projectileSpeed, _projectileSpeedRandomDeviation);
            ProjectileDamage = new(_projectileDamage, _projectileDamageRandomDeviation);
            ProjectilesPerShot = new(_projectilesPerShot, _projectilesPerShotRandomDeviation, MinProjectilesPerShot, MaxProjectilesPerShot * 2);
            FireDuration = new(_fireDuration, _fireDurationRandomDeviation);
            FireRate = new(_fireRate, _fireRateRandomDeviation);
            Cooldown = new(_cooldown, _cooldownRandomDeviation);
            Dispersion = new(0f, _dispersion);
        }
    }
}