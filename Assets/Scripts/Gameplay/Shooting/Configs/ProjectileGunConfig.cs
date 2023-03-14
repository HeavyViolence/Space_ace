using SpaceAce.Auxiliary;
using SpaceAce.Main.ObjectPooling;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Projectile gun config", menuName = "Space ace/Configs/Shooting/Projectile gun config")]
    public sealed class ProjectileGunConfig : GunConfig
    {
        public const float MinProjectileDamage = 10f;
        public const float MaxProjectileDamage = 1000f;

        public const float MinProjectileSpeed = 10f;
        public const float MaxProjectileSpeed = 30f;

        public const float MinFireRate = 1f;
        public const float MaxFirerate = 10f;

        public const int MinProjectilesPerShot = 1;
        public const int MaxProjectilesPerShot = 10;

        public const float MinDispersion = 0f;
        public const float MaxDispersion = 3f;
        public const float DefaultDispersion = 1f;

        public const float MinConvergenceAngle = 0f;
        public const float MaxConvergenceAngle = 1f;
        public const float DefaultConvergenceAngle = 0.3f;

        [SerializeField] private ObjectPoolEntry _projectile = null;
        [SerializeField] private ObjectPoolEntry _hitEffect = null;

        [SerializeField] private float _damage = MinProjectileDamage;
        [SerializeField] private float _damageRandomDeviation = 0f;

        [SerializeField] private float _projectileSpeed = MinProjectileSpeed;
        [SerializeField] private float _projectileSpeedRandomDeviation = 0f;

        [SerializeField] private float _fireRate = MinFireRate;
        [SerializeField] private float _fireRateRandomDeviation = 0f;

        [SerializeField] private int _projectilesPerShot = MinProjectilesPerShot;
        [SerializeField] private int _projectilesPerShotRandomDeviation = 0;

        [SerializeField] private float _dispersion = DefaultDispersion;
        [SerializeField] private float _convergenceAngle = DefaultConvergenceAngle;

        [SerializeField] private bool _cameraShakeOnShotFiredEnabled = false;

        public string ProjectileAnchorName => _projectile.AnchorName;
        public string HitEffectAnchorName => _hitEffect.AnchorName;

        public RangedFloat Damage { get; private set; }
        public RangedFloat ProjectileSpeed { get; private set; }
        public RangedFloat FireRate { get; private set; }
        public RangedInt ProjectilesPerShot { get; private set; }

        public float Dispersion => _dispersion * AuxMath.RandomNormal;
        public Quaternion Dispersion4D => Quaternion.Euler(0f, 0f, _dispersion);

        public float ConvergenceAngle => _convergenceAngle;
        public Quaternion ConvergenceAngle4D => Quaternion.Euler(0f, 0f, _convergenceAngle);

        public bool CameraShakeOnShotFiredEnabled => _cameraShakeOnShotFiredEnabled;

        private void OnEnable()
        {
            Damage = new(_damage, _damageRandomDeviation);
            ProjectileSpeed = new(_projectileSpeed, _projectileSpeedRandomDeviation);
            FireRate = new(_fireRate, _fireRateRandomDeviation);
            ProjectilesPerShot = new(_projectilesPerShot, _projectilesPerShotRandomDeviation, MinProjectilesPerShot, MaxProjectilesPerShot * 2);
        }

        public void EnsureNecessaryObjectPoolsExistence()
        {
            _projectile.EnsureObjectPoolExistence();
            _hitEffect.EnsureObjectPoolExistence();
        }
    }
}