using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.UI;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public abstract class Health : MonoBehaviour, IExperienceSource, IHealthView
    {
        private const float DeathEffectLifetime = 3f;

        private static readonly GameServiceFastAccess<CameraShaker> s_cameraShaker = new();
        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();

        public event EventHandler Depleted, Restored;
        public event EventHandler<FloatValueChangedEventArgs> ValueChanged, MaxValueChanged, RegenerationPerSecondValueChanged;

        [SerializeField] private HealthConfig _config = null;

        private bool _restoredEventToBeCalled = false;
        private float _value;
        private float _maxValue;
        private float _regenPerSecond;

        public float ValuePercentage => Value / MaxValue * 100f;
        public bool Full => Value > MaxValue;
        public float RegainedValue { get; private set; }

        public float Value
        {
            get => _value;

            protected set
            {
                float oldValue = _value;

                _value = Mathf.Clamp(value, 0f, MaxValue);
                ValueChanged?.Invoke(this, new(oldValue, _value));
            }
        }

        public float MaxValue
        {
            get => _maxValue;

            protected set
            {
                float oldValue = _maxValue;

                _maxValue = Mathf.Clamp(value, 0f, float.MaxValue);
                MaxValueChanged?.Invoke(this, new(oldValue, _maxValue));
            }
        }

        public float RegenPerSecond
        {
            get => _regenPerSecond;

            protected set
            {
                float oldValue = _regenPerSecond;

                _regenPerSecond = Mathf.Clamp(value, 0f, float.MaxValue);
                RegenerationPerSecondValueChanged?.Invoke(this, new(oldValue, _regenPerSecond));
            }
        }

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
            RestoreHealthIfRegenerationIsEnabled();
        }

        protected virtual float GetRegenPerSecond() => _config.Regeneration.RandomValue;

        private void RestoreHealthIfRegenerationIsEnabled()
        {
            if (_config.RegenerationEnabled && Full == false)
            {
                float regainedValue = RegenPerSecond * Time.deltaTime;

                Value += regainedValue;
                RegainedValue += regainedValue;

                if (Full && _restoredEventToBeCalled)
                {
                    Restored?.Invoke(this, EventArgs.Empty);

                    _restoredEventToBeCalled = false;
                    Value = MaxValue;
                }
            }
        }

        public void DealDamage(float damage)
        {
            Value -= damage;
            _restoredEventToBeCalled = true;

            if (_config.CameraShakeOnDamagedEnabled) s_cameraShaker.Access.ShakeOnHit();
            if (Value == 0f) Die();
        }

        protected virtual void Die()
        {
            GameObject deathEffect = s_multiobjectPool.Access.GetObject(_config.DeathEffectAnchorName);
            deathEffect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            s_multiobjectPool.Access.ReleaseObject(_config.DeathEffectAnchorName, deathEffect, () => true, DeathEffectLifetime);

            _config.DeathAudio.PlayRandomAudioClip(transform.position);
            s_cameraShaker.Access.ShakeOnDeath();

            Depleted?.Invoke(this, EventArgs.Empty);
        }

        public float GetExperience() => MaxValue + RegainedValue;
    }
}