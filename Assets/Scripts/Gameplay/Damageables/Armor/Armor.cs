using SpaceAce.Gameplay.Experience;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public abstract class Armor : MonoBehaviour, IExperienceSource
    {
        [SerializeField] private ArmorConfig _config;

        public bool Enabled => _config.ArmorEnabled;
        public float Value { get; protected set; }
        public float BlockedDamage { get; private set; }

        protected virtual void OnEnable()
        {
            Value = _config.Armor.RandomValue;
            BlockedDamage = 0f;
        }

        public float GetDamageToBeDealt(float receivedDamage)
        {
            if (receivedDamage < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(receivedDamage), receivedDamage, $"Incoming damage must be positive!");
            }

            if (Enabled == false)
            {
                return receivedDamage;
            }

            float damageToBeDealt;

            if (receivedDamage < Value)
            {
                damageToBeDealt = receivedDamage * receivedDamage / Value;
            }
            else
            {
                damageToBeDealt = receivedDamage;
            }

            BlockedDamage += receivedDamage - damageToBeDealt;

            return damageToBeDealt;
        }

        public float GetExperience() => Value + BlockedDamage;
    }
}