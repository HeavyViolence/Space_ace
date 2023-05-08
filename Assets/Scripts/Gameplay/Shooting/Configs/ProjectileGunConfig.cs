using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Movement;
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

        public const float MinTopSpeed = 20f;
        public const float MaxTopSpeed = 200f;

        public const float MinTopSpeedGainDuration = 0.01f;
        public const float MaxTopSpeedGainDuration = 10f;

        public const float MinDamage = 10f;
        public const float MaxDamage = 1000f;

        public const int MinProjectilesPerShot = 1;
        public const int MaxProjectilesPerShot = 20;

        public const float MinFireDuration = 0.1f;
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
        [SerializeField] private ObjectPoolEntry _hitEffect;

        [SerializeField] private ProjectileBehaviour _behaviour;
        [SerializeField] private TargetSupplier _targetSupplier;

        [SerializeField] private float _topSpeed = MinTopSpeed;
        [SerializeField] private float _topSpeedRandomDeviation = 0f;

        [SerializeField] private float _topSpeedGainDuration = MinTopSpeedGainDuration;
        [SerializeField] private float _topSpeedGainDurationRandomDeviation = 0f;

        [SerializeField] private RotationConfig _rotationConfig;

        [SerializeField] private float _damage = MinDamage;
        [SerializeField] private float _damageRandomDeviation = 0f;

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
        [SerializeField] private AudioCollection _hitAudio;

        [SerializeField] private bool _cameraShakeOnShot = false;

        public int GunGroupID => _gunGroupID;

        public ObjectPoolEntry Projectile => _projectile;
        public ObjectPoolEntry HitEffect => _hitEffect;

        public MovementBehaviour MovementBehaviour => _behaviour.Behaviour;
        public TargetSupplier TargetSupplier => _targetSupplier;
        public RotationConfig RotationConfig => _rotationConfig;

        public RangedFloat TopSpeed { get; private set; }
        public RangedFloat TopSpeedGainDuration { get; private set; }
        public RangedFloat Damage { get; private set; }
        public RangedInt ProjectilesPerShot { get; private set; }
        public RangedFloat FireDuration { get; private set; }
        public RangedFloat FireRate { get; private set; }
        public RangedFloat Cooldown { get; private set; }
        public RangedFloat Dispersion { get; private set; }

        public AudioCollection FireAudio => _fireAudio;
        public AudioCollection HitAudio => _hitAudio;

        public bool CameraShakeOnShotEnabled => _cameraShakeOnShot;

        public float GetConvergenceAngle(bool isRightHandedGun) => isRightHandedGun ? -1f * _convergenceAngle : _convergenceAngle;

        private void OnEnable()
        {
            ApplySettings();
        }

        public void EnsureNecessaryObjectPoolsExistence()
        {
            _projectile.EnsureObjectPoolExistence();
            _hitEffect.EnsureObjectPoolExistence();
        }

        public void ApplySettings()
        {
            TopSpeed = new(_topSpeed, _topSpeedRandomDeviation);
            TopSpeedGainDuration = new(_topSpeedGainDuration, _topSpeedGainDurationRandomDeviation);
            Damage = new(_damage, _damageRandomDeviation);
            ProjectilesPerShot = new(_projectilesPerShot, _projectilesPerShotRandomDeviation, MinProjectilesPerShot, MaxProjectilesPerShot * 2);
            FireDuration = new(_fireDuration, _fireDurationRandomDeviation);
            FireRate = new(_fireRate, _fireRateRandomDeviation);
            Cooldown = new(_cooldown, _cooldownRandomDeviation);
            Dispersion = new(0f, _dispersion);
        }
    }
}