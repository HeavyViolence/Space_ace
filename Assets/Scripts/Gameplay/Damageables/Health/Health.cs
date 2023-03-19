using SpaceAce.Architecture;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public abstract class Health : MonoBehaviour, IExperienceSource
    {
        private const float DeathEffectLifetime = 3f;

        private static readonly GameServiceFastAccess<CameraShaker> s_cameraShaker = new();
        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();

        public event EventHandler Depleted, Restored;

        [SerializeField] private HealthConfig _config = null;

        private bool _restoredEventToBeCalled = false;

        public float Value { get; private set; }
        public float MaxValue { get; private set; }
        public float ValuePercentage => Value / MaxValue;
        public float RegenPerSecond { get; private set; }
        public bool ValueIsFull => Value > MaxValue;
        public float RegainedValue { get; private set; }

        protected virtual void OnEnable()
        {
            MaxValue = _config.HealthCeiling.RandomValue;
            Value = MaxValue;
            RegenPerSecond = GetRegenPerSecond();
            RegainedValue = 0f;
        }

        protected virtual void OnDisable()
        {
            Depleted = null;
            Restored = null;
        }

        protected virtual void Awake()
        {
            _config.EnsureDeathEffectObjectPoolExistence();
        }

        protected virtual void Update()
        {
            RestoreHealthIfRegenIsEnabled();
        }

        protected virtual float GetRegenPerSecond() => _config.Regeneration.RandomValue;

        private void RestoreHealthIfRegenIsEnabled()
        {
            if (_config.RegenerationEnabled && ValueIsFull == false)
            {
                float regainedValue = RegenPerSecond * Time.deltaTime;

                Value += regainedValue;
                RegainedValue += regainedValue;

                if (ValueIsFull && _restoredEventToBeCalled)
                {
                    Restored?.Invoke(this, EventArgs.Empty);

                    _restoredEventToBeCalled = false;
                    Value = MaxValue;
                }
            }
        }

        public void DealDamage(float damage)
        {
            if (damage <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), damage, $"Damage value must be positive!");
            }

            Value -= damage;
            _restoredEventToBeCalled = true;

            if (_config.CameraShakeOnDamagedEnabled)
            {
                s_cameraShaker.Access.ShakeOnHit();
            }

            if (Value < 0f)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            GameObject deathEffect = s_multiobjectPool.Access.GetObject(_config.DeathEffectAnchorName);
            deathEffect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            s_multiobjectPool.Access.ReleaseObject(_config.DeathEffectAnchorName, deathEffect, DeathEffectLifetime);

            _config.DeathAudio.PlayRandomAudioClip(transform.position);
            s_cameraShaker.Access.ShakeOnDeath();

            Depleted?.Invoke(this, EventArgs.Empty);
        }

        public float GetExperience() => MaxValue + RegainedValue;
    }
}