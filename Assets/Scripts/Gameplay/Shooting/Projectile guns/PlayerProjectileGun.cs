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
                                                             IWeaponCoolantUser,
                                                             IWeaponAccelerantUser
    {
        private Coroutine _plasmaShield = null;
        private float _plasmaShieldProjectileSlowdown = 1f;

        private Coroutine _homingAmmo = null;
        private float _homingSpeed = 0f;

        private Coroutine _antimatterAmmo = null;
        private float _antimatterAmmoDamageFactor = 1f;
        private float _antimatterAmmoConsecutiveDamageFactor = 1f;

        private Coroutine _weaponCoolant = null;
        private float _weaponCoolantCooldownReduction = 1f;
        private float _weaponCoolantFireRateBoost = 1f;

        private Coroutine _weaponAccelerant = null;
        private float _weaponAccelerantAmmoSpeedBoost = 1f;
        private float _weaponAccelerantDamageBoost = 1f;
        private float _weaponAccelerantCooldownIncrease = 1f;

        protected override float NextProjectileTopSpeed
        {
            get
            {
                float result = base.NextProjectileTopSpeed;

                if (_plasmaShieldProjectileSlowdown != 1f) result *= _plasmaShieldProjectileSlowdown;
                if (_weaponAccelerantAmmoSpeedBoost != 1f) result *= _weaponAccelerantAmmoSpeedBoost;

                return result;
            }
        }

        protected override float NextProjectileTargetSeekingSpeed => _homingSpeed == 0f ? base.NextProjectileTargetSeekingSpeed
                                                                                        : _homingSpeed;

        protected override float NextCooldown
        {
            get
            {
                float result = base.NextCooldown;

                if (_weaponCoolantCooldownReduction != 1f) result *= _weaponCoolantCooldownReduction;
                if (_weaponAccelerantCooldownIncrease != 1f) result *= _weaponAccelerantCooldownIncrease;

                return result;
            }
        }

        protected override float NextFireRate => _weaponCoolantFireRateBoost == 1f ? base.NextFireRate
                                                                                   : base.NextFireRate * _weaponCoolantFireRateBoost;

        protected override float NextProjectileDamage
        {
            get
            {
                float result = base.NextProjectileDamage;

                if (_weaponAccelerantDamageBoost != 1f) result *= _weaponAccelerantDamageBoost;

                if (_antimatterAmmo != null && TheSameEntityIsHit)
                {
                    _antimatterAmmoDamageFactor *= _antimatterAmmoConsecutiveDamageFactor;
                    result *= _antimatterAmmoDamageFactor;
                }
                else
                {
                    _antimatterAmmoDamageFactor = 1f;
                }

                return result;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_plasmaShield != null)
            {
                StopCoroutine(_plasmaShield);
                _plasmaShield = null;
                _plasmaShieldProjectileSlowdown = 1f;
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

            if (_weaponAccelerant != null)
            {
                StopCoroutine(_weaponAccelerant);
                _weaponAccelerant = null;
                _weaponAccelerantAmmoSpeedBoost = 1f;
                _weaponAccelerantDamageBoost = 1f;
                _weaponAccelerantCooldownIncrease = 1f;
            }
        }

        public bool Use(PlasmaShield shield)
        {
            if (shield is null) throw new ArgumentNullException(nameof(shield));

            if (_plasmaShield == null)
            {
                _plasmaShield = StartCoroutine(ApplySlowProjectiles(shield));
                return true;
            }

            return false;
        }

        private IEnumerator ApplySlowProjectiles(PlasmaShield shield)
        {
            _plasmaShieldProjectileSlowdown = 1f - shield.ProjectilesSlowdown;

            float timer = 0f;

            while (timer < shield.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _plasmaShieldProjectileSlowdown = 1f;
            _plasmaShield = null;
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

        public bool Use(WeaponAccelerant accelerant)
        {
            if (accelerant is null) throw new ArgumentNullException(nameof(accelerant));

            if (_weaponAccelerant == null)
            {
                _weaponAccelerant = StartCoroutine(ApplyWeaponAccelerant(accelerant));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyWeaponAccelerant(WeaponAccelerant accelerant)
        {
            _weaponAccelerantAmmoSpeedBoost = 1f + accelerant.AmmoSpeedBoost;
            _weaponAccelerantDamageBoost = 1f + accelerant.DamageBoost;
            _weaponAccelerantCooldownIncrease = 1f + accelerant.CooldownIncrease;

            float timer = 0f;

            while (timer < accelerant.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _weaponAccelerant = null;
            _weaponAccelerantAmmoSpeedBoost = 1f;
            _weaponAccelerantDamageBoost = 1f;
            _weaponAccelerantCooldownIncrease = 1f;
        }
    }
}