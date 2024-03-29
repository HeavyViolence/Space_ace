using UnityEngine;
using System.Collections;
using SpaceAce.Architecture;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Gameplay.Inventories;
using System;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class ProjectileGun : MonoBehaviour, IGun
    {
        private const float HitEffectDuration = 3f;

        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<CameraShaker> s_cameraShaker = new();
        private static readonly GameServiceFastAccess<SpecialEffectsMediator> s_specialEffectsMediator = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();

        public event EventHandler ShotFired, Hit;

        [SerializeField] private ProjectileGunConfig _config;

        private float _cooldownTimer = Mathf.Infinity;
        private float _currentCooldown = 0f;
        private Coroutine _firingRoutine;

        private Guid _previousEntityHitID = Guid.Empty;
        private Guid _lastEntityHitID = Guid.Empty;

        public virtual float MaxDamagePerSecond => _config.Damage.MaxValue *
                                                   _config.ProjectilesPerShot.MaxValue *
                                                   _config.FireRate.MaxValue *
                                                   _config.FireDuration.MaxValue * 2f /
                                                   (_config.FireDuration.MaxValue + _config.Cooldown.MinValue);

        public bool ReadyToFire => _firingRoutine == null && _cooldownTimer > _currentCooldown;
        public bool CoolingDown => _firingRoutine == null && _cooldownTimer < _currentCooldown;
        public bool IsFiring => _firingRoutine != null;
        public int GroupID => _config.GunGroupID;
        protected bool IsRightHandedGun => transform.localPosition.x > 0f;
        protected virtual float NextProjectileTopSpeed => _config.TopSpeed.RandomValue;
        protected virtual float NextProjectileTopSpeedGainDuration => _config.TopSpeedGainDuration.RandomValue;
        protected virtual float NextProjectileRevolutionsPerMinute => _config.RotationConfig.RevolutionsPerMinute.RandomValue;
        protected virtual float NextProjectileTargetSeekingSpeed => _config.RotationConfig.TargetSeekingSpeed.RandomValue;
        protected virtual float NextProjectileDamage => _config.Damage.RandomValue;
        protected virtual int NextProjectilesPerShot => _config.ProjectilesPerShot.RandomValue;
        protected virtual float NextFireDuration => _config.FireDuration.RandomValue;
        protected virtual float NextFireRate => _config.FireRate.RandomValue;
        protected virtual float NextCooldown => _config.Cooldown.RandomValue;
        protected virtual float NextDispersion => _config.Dispersion.RandomValue;
        protected virtual float ConvergenceAngle => _config.GetConvergenceAngle(IsRightHandedGun);
        protected virtual MovementBehaviour ProjectileBehaviour { get; set; }
        protected virtual Func<Vector2, Transform> TargetSupplier { get; set; }
        protected virtual bool CanFireNextShot => true;
        protected bool TheSameEntityIsHit => _previousEntityHitID.Equals(Guid.Empty) == false &&
                                             _lastEntityHitID.Equals(Guid.Empty) == false &&
                                             _previousEntityHitID.Equals(_lastEntityHitID) == true;

        protected virtual void OnEnable()
        {
            s_specialEffectsMediator.Access.Register(this);
        }

        protected virtual void OnDisable()
        {
            s_specialEffectsMediator.Access.Deregister(this);

            _previousEntityHitID = Guid.Empty;
            _lastEntityHitID = Guid.Empty;
        }

        protected virtual void Awake()
        {
            _config.EnsureNecessaryObjectPoolsExistence();

            ProjectileBehaviour = _config.MovementBehaviour;
            TargetSupplier = _config.TargetSupplier;
        }

        protected virtual void Update()
        {
            if (CoolingDown && GamePauser.Access.Paused == false) _cooldownTimer += Time.deltaTime;
        }

        public bool Fire()
        {
            if (ReadyToFire)
            {
                _firingRoutine = StartCoroutine(FiringRoutine());
                return true;
            }

            return false;
        }

        public bool StopFire()
        {
            if (IsFiring)
            {
                StopCoroutine(_firingRoutine);
                _firingRoutine = null;

                return true;
            }

            return false;
        }

        private IEnumerator FiringRoutine()
        {
            int shotsToFire = Mathf.RoundToInt(NextFireRate * NextFireDuration);
            Transform target = TargetSupplier?.Invoke(transform.position);

            for (int i = 0; i < shotsToFire; i++)
            {
                while (GamePauser.Access.Paused == true) yield return null;

                if (CanFireNextShot)
                {
                    int projectilesPerShot = NextProjectilesPerShot;

                    for (int y = 0; y < projectilesPerShot; y++) PerformShot(target);

                    _config.FireAudio.PlayRandomAudioClip(transform.position);
                }

                yield return new WaitForSeconds(1f / NextFireRate);
            }

            _cooldownTimer = 0f;
            _currentCooldown = NextCooldown;
            _firingRoutine = null;
        }

        private void PerformShot(Transform target)
        {
            var projectile = s_multiobjectPool.Access.GetObject(_config.Projectile.AnchorName);
            float dispersion = NextDispersion;

            Vector2 projectileDirection = new(ConvergenceAngle + dispersion, transform.up.y);
            Quaternion projectileRotation = transform.rotation * Quaternion.Euler(0f, 0f, dispersion);

            projectile.transform.SetPositionAndRotation(transform.position, projectileRotation);

            s_multiobjectPool.Access.ReleaseObject(_config.Projectile.AnchorName,
                                                   projectile,
                                                   () => s_masterCameraHolder.Access.InsideViewport(projectile.transform.position) == false);

            MovementBehaviourSettings settings = new(projectileDirection,
                                                     NextProjectileTopSpeed,
                                                     NextProjectileTopSpeedGainDuration,
                                                     NextProjectileRevolutionsPerMinute,
                                                     target,
                                                     NextProjectileTargetSeekingSpeed);

            SupplyProjectileBehaviour(projectile, ProjectileBehaviour, settings);
            AwaitProjectileHit(projectile);

            if (_config.CameraShakeOnShotEnabled) s_cameraShaker.Access.ShakeOnShotFired();

            ShotFired?.Invoke(this, EventArgs.Empty);
        }

        private void SupplyProjectileBehaviour(GameObject projectile, MovementBehaviour behaviour, MovementBehaviourSettings settings)
        {
            if (projectile.TryGetComponent(out IMovementBehaviourSupplier supplier) == true) supplier.SupplyMovementBehaviour(behaviour, settings);
            else throw new MissingComponentException($"Projectile doesn't have a mandatory {typeof(IMovementBehaviourSupplier)} component!");
        }

        private void AwaitProjectileHit(GameObject projectile)
        {
            if (projectile.TryGetComponent(out DamageDealer dealer) == true)
            {
                dealer.Hit += (s, e) =>
                {
                    _previousEntityHitID = _lastEntityHitID;
                    _lastEntityHitID = e.DamageReceiver.ID;

                    e.DamageReceiver.ApplyDamage(NextProjectileDamage);

                    s_multiobjectPool.Access.ReleaseObject(_config.Projectile.AnchorName, projectile, () => true);

                    var hitEffect = s_multiobjectPool.Access.GetObject(_config.HitEffect.AnchorName);
                    hitEffect.transform.position = e.HitPosition;

                    s_multiobjectPool.Access.ReleaseObject(_config.HitEffect.AnchorName, hitEffect, () => true, HitEffectDuration);

                    if (_config.HitAudio != null) _config.HitAudio.PlayRandomAudioClip(e.HitPosition);

                    Hit?.Invoke(this, EventArgs.Empty);
                };
            }
            else
            {
                throw new MissingComponentException($"Projectile doesn't have a mandatory {typeof(DamageDealer)} component!");
            }
        }
    }
}