using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class PlayerShipHealth : Health, IRepairKitUser, IReactiveArmorUser
    {
        private Coroutine _repairKitRoutine = null;
        private Coroutine _reactiveArmorRoutine = null;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_repairKitRoutine != null)
            {
                StopCoroutine(_repairKitRoutine);
                _repairKitRoutine = null;
            }

            if (_reactiveArmorRoutine != null)
            {
                StopCoroutine(_reactiveArmorRoutine);
                _reactiveArmorRoutine = null;
            }
        }

        public bool Use(RepairKit kit)
        {
            if (kit is null) throw new ArgumentNullException(nameof(kit));

            if (_repairKitRoutine == null)
            {
                _repairKitRoutine = StartCoroutine(ApplyRepairKit(kit));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyRepairKit(RepairKit kit)
        {
            RegenPerSecond += kit.RegenPerSecond;
            float timer = 0f;

            while (timer < kit.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            RegenPerSecond -= kit.RegenPerSecond;
            _repairKitRoutine = null;
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
            MaxValue += armor.HealthIncrease;
            Value = MaxValue;

            float timer = 0f;

            while (timer < armor.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            MaxValue -= armor.HealthIncrease;
            Value = MaxValue;

            _reactiveArmorRoutine = null;
        }
    }
}