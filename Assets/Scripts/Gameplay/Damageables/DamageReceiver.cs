using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Main;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(ExperienceHolder))]
    public sealed class DamageReceiver : MonoBehaviour
    {
        public event EventHandler<DamageReceivedEventArgs> DamageReceived;
        public event EventHandler<DestroyedEventArgs> Destroyed;

        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();

        private Health _health;
        private Armor _armor;
        private ExperienceHolder _experience;
        private float _lifetime = 0f;

        public string ID { get; private set; }

        private void Awake()
        {
            ID = StringID.NextCryptosafe();

            CacheRequiredComponents();
        }

        private void OnEnable()
        {
            _health.Depleted += HealthDepletedEventHandler;

            _lifetime = 0f;
        }

        private void OnDisable()
        {
            _health.Depleted -= HealthDepletedEventHandler;

            DamageReceived = null;
            Destroyed = null;
        }

        private void Update()
        {
            if (s_masterCameraHolder.Access.InsideViewport(transform.position) == true)
            {
                _lifetime += Time.deltaTime;
            }
        }

        private void CacheRequiredComponents()
        {
            if (TryGetComponent(out Health health))
            {
                _health = health;
            }
            else
            {
                throw new MissingComponentException($"{name} is missing a mandatory component of type {typeof(Health)}!");
            }

            if (TryGetComponent(out Armor armor))
            {
                _armor = armor;
            }
            else
            {
                throw new MissingComponentException($"{name} is missing a mandatory component of type {typeof(Armor)}!");
            }

            _experience = GetComponent<ExperienceHolder>();
        }

        private void HealthDepletedEventHandler(object sender, EventArgs e)
        {
            (float earned, float lost, float total) = _experience.GetValues();

            Destroyed?.Invoke(this, new DestroyedEventArgs(transform.position, _lifetime, earned, lost, total));
        }

        public void ApplyDamage(float damage)
        {
            if (s_masterCameraHolder.Access.InsideViewport(transform.position) == false)
            {
                return;
            }

            float damageToBeDealt = _armor.Enabled ? _armor.GetDamageToBeDealt(damage) : damage;

            _health.DealDamage(damageToBeDealt);
            DamageReceived?.Invoke(this, new DamageReceivedEventArgs(damage, damageToBeDealt, transform.position));
        }
    }
}