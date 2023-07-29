using SpaceAce.Gameplay.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    public sealed class PlayerShipHealth : Health, IRepairKitUser
    {
        private Coroutine _repairKitRoutine = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            SpecialEffectsMediator.Register(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SpecialEffectsMediator.Deregister(this);

            if (_repairKitRoutine != null)
            {
                StopCoroutine(_repairKitRoutine);
                _repairKitRoutine = null;
            }
        }

        public bool Use(RepairKit kit)
        {
            if (kit is null) throw new ArgumentNullException(nameof(kit));

            if (_repairKitRoutine == null)
            {
                _repairKitRoutine = StartCoroutine(Repair(kit));
                return true;
            }

            return false;
        }

        private IEnumerator Repair(RepairKit kit)
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
    }
}