using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Movement;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class PlayerProjectileGun : ProjectileGun, IPlasmaShieldUser, IHomingAmmoUser
    {
        private Coroutine _projectileSlower = null;
        private float _projectileSlowdown = 0f;

        private Coroutine _homingProjectiles = null;
        private float _homingSpeed = 0f;

        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * (1f - _projectileSlowdown);
        protected override float NextProjectileTargetSeekingSpeed => _homingSpeed == 0f ? base.NextProjectileTargetSeekingSpeed : _homingSpeed;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_projectileSlower != null)
            {
                StopCoroutine(_projectileSlower);
                _projectileSlower = null;
                _projectileSlowdown = 0f;
            }

            if (_homingProjectiles != null)
            {
                StopCoroutine(_homingProjectiles);
                _homingProjectiles = null;
                _homingSpeed = 0f;
            }
        }

        public bool Use(PlasmaShield shield)
        {
            if (shield is null) throw new ArgumentNullException(nameof(shield));

            if (_projectileSlower == null)
            {
                _projectileSlower = StartCoroutine(SlowProjectiles(shield));
                return true;
            }

            return false;
        }

        private IEnumerator SlowProjectiles(PlasmaShield shield)
        {
            _projectileSlowdown -= shield.ProjectilesSlowdown;
            float timer = 0f;

            while (timer < shield.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _projectileSlowdown = 1f;
            _projectileSlower = null;
        }

        public bool Use(HomingAmmo ammo)
        {
            if (ammo is null) throw new ArgumentNullException(nameof(ammo));

            if (_homingProjectiles == null)
            {
                _homingProjectiles = StartCoroutine(HomingProjectiles(ammo));
                return true;
            }

            return false;
        }

        private IEnumerator HomingProjectiles(HomingAmmo ammo)
        {
            _homingSpeed = ammo.HomingSpeed;

            MovementBehaviour defaultBehaviour = ProjectileBehaviour;
            ProjectileBehaviour = ammo.AmmoBehaviour;

            Func<Vector2, Transform> defaultTargetSupplier = TargetSupplier;
            TargetSupplier = ammo.TargetLocator;

            float timer = 0f;

            while (timer < ammo.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _homingSpeed = 0f;
            ProjectileBehaviour = defaultBehaviour;
            TargetSupplier = defaultTargetSupplier;
            _homingProjectiles = null;
        }
    }
}