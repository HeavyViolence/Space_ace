using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class PlayerShipArmor : Armor, IPlasmaShieldUser, IReactiveArmorUser
    {
        private Coroutine _plasmaShieldRoutine = null;

        private Coroutine _reactiveArmorRoutine = null;
        private float _damageToArmorConversionRate = 0f;

        protected override void OnEnable()
        {
            base.OnEnable();

            SpecialEffectsMediator.Register(this);
        }

        private void OnDisable()
        {
            SpecialEffectsMediator.Deregister(this);

            if (_plasmaShieldRoutine != null)
            {
                StopCoroutine(_plasmaShieldRoutine);
                _plasmaShieldRoutine = null;
            }

            if (_reactiveArmorRoutine != null)
            {
                StopCoroutine(_reactiveArmorRoutine);
                _reactiveArmorRoutine = null;
                _damageToArmorConversionRate = 0f;
            }
        }

        public override float GetDamageToBeDealt(float receivedDamage)
        {
            if (_damageToArmorConversionRate > 0f) Value += receivedDamage * _damageToArmorConversionRate;

            return base.GetDamageToBeDealt(receivedDamage);
        }

        public bool Use(PlasmaShield shield)
        {
            if (shield is null) throw new ArgumentNullException(nameof(shield));

            if (_plasmaShieldRoutine == null)
            {
                _plasmaShieldRoutine = StartCoroutine(ApplyPlasmaShield(shield));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyPlasmaShield(PlasmaShield shield)
        {
            Value += shield.ArmorBoost;
            float timer = 0f;

            while (timer < shield.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            Value -= shield.ArmorBoost;
            _plasmaShieldRoutine = null;
        }

        public bool Use(ReactiveArmor armor)
        {
            if (armor is null) throw new ArgumentNullException(nameof(armor));

            if (_reactiveArmorRoutine == null)
            {
                _reactiveArmorRoutine = StartCoroutine(ApplyReactiveArmor(armor));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyReactiveArmor(ReactiveArmor armor)
        {
            float timer = 0f;
            float initialValue = Value;
            _damageToArmorConversionRate = armor.DamageToArmorConversionRate;

            while (timer < armor.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _reactiveArmorRoutine = null;
            _damageToArmorConversionRate = 0f;
            Value = initialValue;
        }
    }
}