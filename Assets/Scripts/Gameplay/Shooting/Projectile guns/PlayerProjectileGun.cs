using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Movement;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class PlayerProjectileGun : ProjectileGun, IPlasmaShieldUser,
                                                             IHomingAmmoUser,
                                                             IAntimatterAmmoUser
    {
        private Coroutine _slowProjectiles = null;
        private float _projectileSlowdown = 0f;

        private Coroutine _homingAmmo = null;
        private float _homingSpeed = 0f;

        private Coroutine _antimatterAmmo = null;
        private float _antimatterAmmoDamageFactor = 1f;
        private float _antimatterAmmoConsecutiveDamageFactor = 1f;
        private string _previousHitID = string.Empty;

        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * (1f - _projectileSlowdown);
        protected override float NextProjectileTargetSeekingSpeed => _homingSpeed == 0f ? base.NextProjectileTargetSeekingSpeed : _homingSpeed;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_slowProjectiles != null)
            {
                StopCoroutine(_slowProjectiles);
                _slowProjectiles = null;
                _projectileSlowdown = 0f;
            }

            if (_homingAmmo != null)
            {
                StopCoroutine(_homingAmmo);
                _homingAmmo = null;
                _homingSpeed = 0f;
            }

            if (_antimatterAmmo != null)
            {
                StopCoroutine(_antimatterAmmo);
                _antimatterAmmo = null;
                _antimatterAmmoDamageFactor = 1f;
                _antimatterAmmoConsecutiveDamageFactor = 1f;
            }
        }

        public bool Use(PlasmaShield shield)
        {
            if (shield is null) throw new ArgumentNullException(nameof(shield));

            if (_slowProjectiles == null)
            {
                _slowProjectiles = StartCoroutine(ApplySlowProjectiles(shield));
                return true;
            }

            return false;
        }

        private IEnumerator ApplySlowProjectiles(PlasmaShield shield)
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
            _slowProjectiles = null;
        }

        public bool Use(HomingAmmo ammo)
        {
            if (ammo is null) throw new ArgumentNullException(nameof(ammo));

            if (_homingAmmo == null)
            {
                _homingAmmo = StartCoroutine(ApplyHomingAmmo(ammo));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyHomingAmmo(HomingAmmo ammo)
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
            _homingAmmo = null;
        }

        public bool Use(AntimatterAmmo ammo)
        {
            if (ammo is null) throw new ArgumentNullException(nameof(ammo));

            if (_antimatterAmmo == null)
            {
                _antimatterAmmo = StartCoroutine(ApplyAntimatterAmmo(ammo));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyAntimatterAmmo(AntimatterAmmo ammo)
        {
            _antimatterAmmoDamageFactor = 1f;
            _antimatterAmmoConsecutiveDamageFactor = ammo.ConsecutiveDamageFactor;

            float timer = 0f;

            while (timer < ammo.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _antimatterAmmoDamageFactor = 1f;
            _antimatterAmmoConsecutiveDamageFactor = 1f;
            _antimatterAmmo = null;
        }

        protected override float GetNextProjectileDamage(string hitID)
        {
            if (_antimatterAmmo == null)
            {
                return NextProjectileDamage;
            }
            else
            {
                if (hitID == _previousHitID)
                {
                    _antimatterAmmoDamageFactor *= _antimatterAmmoConsecutiveDamageFactor;
                    _previousHitID = hitID;

                    return NextProjectileDamage * _antimatterAmmoDamageFactor;
                }
                else
                {
                    _antimatterAmmoDamageFactor = 1f;
                    _previousHitID = hitID;

                    return NextProjectileDamage;
                }
            }
        }
    }
}