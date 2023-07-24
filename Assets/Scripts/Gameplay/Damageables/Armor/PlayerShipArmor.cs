using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class PlayerShipArmor : Armor, IPlasmaShieldUser
    {
        private Coroutine _armorBooster = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            SpecialEffectsMediator.Register(this);
        }

        private void OnDisable()
        {
            SpecialEffectsMediator.Deregister(this);

            if (_armorBooster != null)
            {
                StopCoroutine(_armorBooster);
                _armorBooster = null;
            }
        }

        public bool Use(PlasmaShield shield)
        {
            if (shield is null) throw new ArgumentNullException(nameof(shield));

            if (_armorBooster == null)
            {
                _armorBooster = StartCoroutine(BoostArmor(shield));
                return true;
            }

            return false;
        }

        private IEnumerator BoostArmor(PlasmaShield shield)
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
            _armorBooster = null;
        }
    }
}