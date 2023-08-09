using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Movement;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class PlayerProjectileGun : ProjectileGun, IPlasmaShieldUser,
                                                             IHomingAmmoUser,
                                                             IAntimatterAmmoUser,
                                                             IWeaponCoolantUser
    {
        private Coroutine _slowProjectiles = null;
        private float _projectileSlowdown = 1f;

        private Coroutine _homingAmmo = null;
        private float _homingSpeed = 0f;

        private Coroutine _antimatterAmmo = null;
        private float _antimatterAmmoDamageFactor = 1f;
        private float _antimatterAmmoConsecutiveDamageFactor = 1f;
        private string _previousHitID = string.Empty;

        private Coroutine _weaponCoolant = null;
        private float _weaponCoolantCooldownReduction = 1f;
        private float _weaponCoolantFireRateBoost = 1f;

        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * _projectileSlowdown;

        protected override float NextProjectileTargetSeekingSpeed => _homingSpeed == 0f ? base.NextProjectileTargetSeekingSpeed
                                                                                        : _homingSpeed;

        protected override float NextCooldown => _weaponCoolantCooldownReduction == 1f ? base.NextCooldown
                                                                                       : base.NextCooldown * _weaponCoolantCooldownReduction;

        protected override float NextFireRate => _weaponCoolantFireRateBoost == 1f ? base.NextFireRate
                                                                                   : base.NextFireRate * _weaponCoolantFireRateBoost;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_slowProjectiles != null)
            {
                StopCoroutine(_slowProjectiles);
                _slowProjectiles = null;
                _projectileSlowdown = 1f;
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

            if (_weaponCoolant != null)
            {
                StopCoroutine(_weaponCoolant);
                _weaponCoolant = null;
                _weaponCoolantCooldownReduction = 1f;
                _weaponCoolantFireRateBoost = 1f;
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
            _projectileSlowdown = 1f - shield.ProjectilesSlowdown;

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

        public bool Use(WeaponCoolant coolant)
        {
            if (coolant is null) throw new ArgumentNullException(nameof(coolant));

            if (_weaponCoolant == null)
            {
                _weaponCoolant = StartCoroutine(ApplyWeaponCoolant(coolant));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyWeaponCoolant(WeaponCoolant coolant)
        {
            _weaponCoolantCooldownReduction = 1f - coolant.CooldownReduction;
            _weaponCoolantFireRateBoost = 1f + coolant.FireRateBoost;

            float timer = 0f;

            while (timer < coolant.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _weaponCoolantCooldownReduction = 1f;
            _weaponCoolantFireRateBoost = 1f;
            _weaponCoolant = null;
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