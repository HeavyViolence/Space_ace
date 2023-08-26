using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(ExperienceHolder))]
    public abstract class DamageReceiver : MonoBehaviour, IDamageable, IDestroyable
    {
        public event EventHandler<DamageReceivedEventArgs> DamageReceived;
        public event EventHandler BeforeDestroyed;
        public event EventHandler<DestroyedEventArgs> Destroyed;

        private static readonly GameServiceFastAccess<SpecialEffectsMediator> s_specialEffectsMediator = new();
        protected static readonly GameServiceFastAccess<MasterCameraHolder> MasterCameraHolder = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();

        private Health _health;
        private Armor _armor;
        private ExperienceHolder _experience;
        private float _lifetime = 0f;

        public string ID { get; private set; }

        protected virtual void Awake()
        {
            ID = StringID.NextCryptosafe();

            CacheRequiredComponents();
        }

        protected virtual void OnEnable()
        {
            s_specialEffectsMediator.Access.Register(this);

            _health.Depleted += (s, e) => StartCoroutine(DestructionRoutine());

            _lifetime = 0f;
        }

        protected virtual void OnDisable()
        {
            s_specialEffectsMediator.Access.Deregister(this);

            _health.Depleted -= (s, e) => StartCoroutine(DestructionRoutine());

            DamageReceived = null;
            BeforeDestroyed = null;
            Destroyed = null;
        }

        protected virtual void Update()
        {
            if (MasterCameraHolder.Access.InsideViewport(transform.position) == true) _lifetime += Time.deltaTime;
        }

        private void CacheRequiredComponents()
        {
            if (TryGetComponent(out Health health)) _health = health;
            else throw new MissingComponentException($"{name} is missing a mandatory component of type {typeof(Health)}!");

            if (TryGetComponent(out Armor armor)) _armor = armor;
            else throw new MissingComponentException($"{name} is missing a mandatory component of type {typeof(Armor)}!");

            _experience = GetComponent<ExperienceHolder>();
        }

        private IEnumerator DestructionRoutine()
        {
            BeforeDestroyed?.Invoke(this, EventArgs.Empty);

            yield return null;

            (float earned, float lost) = _experience.GetValues();
            Destroyed?.Invoke(this, new(transform.position, _lifetime, earned, lost));
        }

        public virtual void ApplyDamage(float damage)
        {
            float damageToBeDealt = _armor.Enabled ? _armor.GetDamageToBeDealt(damage) : damage;

            _health.DealDamage(damageToBeDealt);
            DamageReceived?.Invoke(this, new DamageReceivedEventArgs(damage, damageToBeDealt, transform.position));
        }
    }
}