using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(SpaceDebrisHealth))]
    [RequireComponent(typeof(SpaceDebrisArmor))]
    public sealed class SpaceDebrisDamageReceiver : DamageReceiver, IAtomizerUser
    {
        public bool Use(Atomizer atomizer)
        {
            if (atomizer is null) throw new ArgumentNullException(nameof(atomizer));

            if (MasterCameraHolder.Access.InsideViewport(transform.position) == true)
            {
                ApplyDamage(float.PositiveInfinity);
                return true;
            }

            return false;
        }
    }
}