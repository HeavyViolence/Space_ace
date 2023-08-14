using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyProjectileGun : ProjectileGun, IAmplifiable, IStasisFieldUser, IEMPUser
    {
        private float _amplificationFactor = 1f;

        private float _stasisFieldSlowdown = 1f;
        private Coroutine _stasisField = null;

        private float _jamProbability = 0f;
        private Coroutine _emp = null;

        public override float MaxDamagePerSecond => base.MaxDamagePerSecond * _amplificationFactor;
        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * _amplificationFactor * _stasisFieldSlowdown;
        protected override float NextProjectileTopSpeedGainDuration => base.NextProjectileTopSpeedGainDuration / _amplificationFactor * _stasisFieldSlowdown;
        protected override float NextProjectileRevolutionsPerMinute => base.NextProjectileRevolutionsPerMinute * _stasisFieldSlowdown;
        protected override float NextProjectileTargetSeekingSpeed => base.NextProjectileTargetSeekingSpeed * _amplificationFactor * _stasisFieldSlowdown;
        protected override float NextProjectileDamage => base.NextProjectileDamage * _amplificationFactor;
        protected override float NextCooldown => base.NextCooldown / _amplificationFactor;
        protected override float NextFireDuration => base.NextFireDuration * _amplificationFactor;
        protected override float NextFireRate => base.NextFireRate * _amplificationFactor;
        protected override bool CanFireNextShot => _jamProbability == 0f || AuxMath.Random > _jamProbability;

        protected override void OnDisable()
        {
            base.OnDisable();

            _amplificationFactor = 1f;

            if (_stasisField != null)
            {
                StopCoroutine(_stasisField);
                _stasisField = null;
                _stasisFieldSlowdown = 1f;
            }

            if (_emp != null)
            {
                StopCoroutine(_emp);
                _emp = null;
                _jamProbability = 0f;
            }
        }

        public void Amplify(float factor) => _amplificationFactor = factor;

        public bool Use(StasisField field)
        {
            if (_stasisField == null)
            {
                _stasisField = StartCoroutine(ApplyStasisField(field));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyStasisField(StasisField field)
        {
            _stasisFieldSlowdown = 1f - field.Slowdown;
            float timer = 0f;

            while (timer < field.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _stasisFieldSlowdown = 1f;
            _stasisField = null;
        }

        public bool Use(EMP emp)
        {
            if (emp is null) throw new ArgumentNullException(nameof(emp));

            if (_emp == null)
            {
                _emp = StartCoroutine(ApplyEMP(emp));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyEMP(EMP emp)
        {
            _jamProbability = emp.JamProbability;
            float timer = 0f;

            while (timer > emp.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _jamProbability = 0f;
            _emp = null;
        }
    }
}