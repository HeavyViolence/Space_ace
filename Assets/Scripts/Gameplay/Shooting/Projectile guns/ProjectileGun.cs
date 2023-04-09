using UnityEngine;
using System.Collections;
using SpaceAce.Architecture;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main;
using SpaceAce.Gameplay.Movement;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class ProjectileGun : MonoBehaviour, IGun
    {
        private const float HitEffectDuration = 2f;

        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<CameraShaker> s_cameraShaker = new();

        [SerializeField] protected ProjectileGunConfig _config;

        private float _cooldownTimer = Mathf.Infinity;
        private float _currentCooldown = 0f;
        private Coroutine _firingRoutine;

        public float MaxDamagePerSecond => _config.ProjectileDamage.MaxValue *
                                           _config.ProjectilesPerShot.MaxValue *
                                           _config.FireRate.MaxValue *
                                           _config.FireDuration.MaxValue * 2f /
                                           (_config.FireDuration.MaxValue + _config.Cooldown.MinValue);
        public bool ReadyToFire => _firingRoutine == null && _cooldownTimer > _currentCooldown;
        public bool CoolingDown => _firingRoutine == null && _cooldownTimer < _currentCooldown;
        public bool IsFiring => _firingRoutine != null;
        public int GunGroupID => _config.GunGroupID;
        protected bool IsRightHandedGun => transform.localPosition.x > 0f;
        protected virtual float NextProjectileSpeed => _config.ProjectileSpeed.RandomValue;
        protected virtual float NextProjectileDamage => _config.ProjectileDamage.RandomValue;
        protected virtual int NextProjectilesPerShot => _config.ProjectilesPerShot.RandomValue;
        protected virtual float NextFireDuration => _config.FireDuration.RandomValue;
        protected virtual float NextFireRate => _config.FireRate.RandomValue;
        protected virtual float NextCooldown => _config.Cooldown.RandomValue;
        protected virtual float NextDispersion => _config.Dispersion.RandomValue;
        protected virtual float ConvergenceAngle => _config.GetConvergenceAngle(IsRightHandedGun);
        protected MovementBehaviour ProjectileMovementBehaviour { get; set; }

        private void Awake()
        {
            _config.EnsureNecessaryObjectPoolsExistence();

            ProjectileMovementBehaviour = delegate (Rigidbody2D body, Vector2 direction, float speed)
            {
                Vector2 velocity = Time.fixedDeltaTime * speed * direction;
                body.MovePosition(body.position + velocity);
            };
        }

        private void Update()
        {
            if (CoolingDown)
            {
                _cooldownTimer += Time.deltaTime;
            }
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

            for (int i = 0; i < shotsToFire; i++)
            {
                int projectilesPerShot = NextProjectilesPerShot;

                for (int y = 0; y < projectilesPerShot; y++)
                {
                    PerformShot();
                }

                _config.FireAudio.PlayRandomAudioClip(transform.position);

                yield return new WaitForSeconds(1f / NextFireRate);
            }

            _cooldownTimer = 0f;
            _currentCooldown = NextCooldown;

            _firingRoutine = null;
        }

        private void PerformShot()
        {
            var projectile = s_multiobjectPool.Access.GetObject(_config.Projectile.AnchorName);

            float dispersion = NextDispersion;
            Vector2 projectileDirection = new(ConvergenceAngle + dispersion, transform.up.y);

            projectile.transform.position = transform.position;
            projectile.transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, dispersion);

            s_multiobjectPool.Access.ReleaseObject(_config.Projectile.AnchorName,
                                                   projectile,
                                                   () => s_masterCameraHolder.Access.InsideViewport(projectile.transform.position) == false);

            SupplyProjectileMovementBehaviour(projectile, ProjectileMovementBehaviour, projectileDirection, NextProjectileSpeed);
            AwaitProjectileHit(projectile);

            if (_config.CameraShakeOnShotEnabled)
            {
                s_cameraShaker.Access.ShakeOnShotFired();
            }
        }

        private void SupplyProjectileMovementBehaviour(GameObject projectile, MovementBehaviour behaviour, Vector2 direction, float speed)
        {
            if (projectile.TryGetComponent(out IMovementBehaviourSupplier supplier) == true)
            {
                supplier.SupplyMovementBehaviour(behaviour, direction, speed);
            }
            else
            {
                throw new MissingComponentException($"Projectile doesn't have mandatory {typeof(IMovementBehaviourSupplier)} component!");
            }
        }

        private void AwaitProjectileHit(GameObject projectile)
        {
            if (projectile.TryGetComponent(out DamageDealer dealer) == true)
            {
                dealer.Hit += (s, e) =>
                {
                    e.DamageReceiver.ApplyDamage(NextProjectileDamage);

                    s_multiobjectPool.Access.ReleaseObject(_config.Projectile.AnchorName, projectile, () => true);

                    var hitEffect = s_multiobjectPool.Access.GetObject(_config.ProjectileHitEffect.AnchorName);
                    hitEffect.transform.position = e.HitPosition;

                    s_multiobjectPool.Access.ReleaseObject(_config.ProjectileHitEffect.AnchorName, hitEffect, () => true, HitEffectDuration);
                };
            }
            else
            {
                throw new MissingComponentException($"Projectile doesn't have mandatory {typeof(DamageDealer)} component!");
            }
        }
    }
}