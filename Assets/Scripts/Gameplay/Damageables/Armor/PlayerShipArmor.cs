using SpaceAce.Gameplay.Inventories;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class PlayerShipArmor : Armor, IPlasmaShieldUser
    {
        private Coroutine _armorBoostingRoutine = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            SpecialEffectsMediator.Register(this);
        }

        private void OnDisable()
        {
            SpecialEffectsMediator.Deregister(this);

            if (_armorBoostingRoutine != null)
            {
                StopCoroutine(_armorBoostingRoutine);
                _armorBoostingRoutine = null;
            }
        }

        public bool Use(PlasmaShield shield)
        {
            if (_armorBoostingRoutine == null)
            {
                _armorBoostingRoutine = StartCoroutine(BoostArmor(shield.ArmorBoost, shield.Duration));
                return true;
            }

            return false;
        }

        private IEnumerator BoostArmor(float boostValue, float duration)
        {
            Value += boostValue;

            yield return new WaitForSeconds(duration);

            Value -= boostValue;
            _armorBoostingRoutine = null;
        }
    }
}