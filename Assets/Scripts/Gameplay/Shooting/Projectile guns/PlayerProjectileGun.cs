using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class PlayerProjectileGun : ProjectileGun, IPlasmaShieldUser
    {
        private Coroutine _projectileSlower = null;
        private float _projectileSpeedFactor = 1f;

        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * _projectileSpeedFactor;

        private void OnEnable()
        {
            SpecialEffectsMediator.Register(this);
        }

        private void OnDisable()
        {
            SpecialEffectsMediator.Deregister(this);

            if (_projectileSlower != null)
            {
                StopCoroutine(_projectileSlower);
                _projectileSlower = null;
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
            _projectileSpeedFactor -= shield.ProjectilesSlowdown;
            float timer = 0f;

            while (timer < shield.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _projectileSpeedFactor = 1f;
            _projectileSlower = null;
        }
    }
}