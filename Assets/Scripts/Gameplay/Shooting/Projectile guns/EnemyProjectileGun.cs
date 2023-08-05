using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Inventories;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyProjectileGun : ProjectileGun, IAmplifiable, IStasisFieldUser
    {
        private float _amplificationFactor = 1f;

        private float _stasisFieldSlowdown = 1f;
        private Coroutine _stasisFieldRoutine = null;

        public override float MaxDamagePerSecond => base.MaxDamagePerSecond * _amplificationFactor;

        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * _amplificationFactor * _stasisFieldSlowdown;
        protected override float NextProjectileTopSpeedGainDuration => base.NextProjectileTopSpeedGainDuration / _amplificationFactor * _stasisFieldSlowdown;
        protected override float NextProjectileRevolutionsPerMinute => base.NextProjectileRevolutionsPerMinute * _stasisFieldSlowdown;
        protected override float NextProjectileTargetSeekingSpeed => base.NextProjectileTargetSeekingSpeed * _amplificationFactor * _stasisFieldSlowdown;
        protected override float NextProjectileDamage => base.NextProjectileDamage * _amplificationFactor;
        protected override float NextCooldown => base.NextCooldown / _amplificationFactor;
        protected override float NextFireDuration => base.NextFireDuration * _amplificationFactor;
        protected override float NextFireRate => base.NextFireRate * _amplificationFactor;

        protected override void OnEnable()
        {
            base.OnEnable();

            _amplificationFactor = 1f;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_stasisFieldRoutine != null)
            {
                StopCoroutine(_stasisFieldRoutine);
                _stasisFieldRoutine = null;
            }
        }

        public void Amplify(float factor) => _amplificationFactor = factor;

        public bool Use(StasisField field)
        {
            if (_stasisFieldRoutine == null)
            {
                _stasisFieldRoutine = StartCoroutine(StasisFieldRoutine(field));
                return true;
            }

            return false;
        }

        private IEnumerator StasisFieldRoutine(StasisField field)
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
            _stasisFieldRoutine = null;
        }
    }
}