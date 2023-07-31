using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.UI;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public abstract class Armor : MonoBehaviour, IExperienceSource, IArmorView
    {
        private static readonly GameServiceFastAccess<SpecialEffectsMediator> s_specialEffectsMediator = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();

        public event EventHandler<FloatValueChangedEventArgs> ValueChanged;

        [SerializeField] private ArmorConfig _config;

        private float _value;

        public float Value
        {
            get => _value;

            protected set
            {
                float oldValue = _value;

                _value = Mathf.Clamp(value, 0f, float.MaxValue);
                ValueChanged?.Invoke(this, new(oldValue, _value));
            }
        }

        public bool Enabled => _config.ArmorEnabled;
        public float BlockedDamage { get; private set; }

        protected virtual void OnEnable()
        {
            Value = _config.Armor.RandomValue;
            BlockedDamage = 0f;

            s_specialEffectsMediator.Access.Register(this);
        }

        protected virtual void OnDisable()
        {
            s_specialEffectsMediator.Access.Deregister(this);
        }

        public virtual float GetDamageToBeDealt(float receivedDamage)
        {
            if (receivedDamage < 0f) throw new ArgumentOutOfRangeException(nameof(receivedDamage));
            if (Enabled == false) return receivedDamage;

            float damageToBeDealt = receivedDamage < Value ? receivedDamage * receivedDamage / Value : receivedDamage;
            BlockedDamage += receivedDamage - damageToBeDealt;

            return damageToBeDealt;
        }

        public float GetExperience() => Value + BlockedDamage;
    }
}