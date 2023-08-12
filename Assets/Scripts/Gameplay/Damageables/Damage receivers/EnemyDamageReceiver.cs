using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(EnemyArmor))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyDamageReceiver : DamageReceiver, IAtomizerUser
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