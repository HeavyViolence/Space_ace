using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(SpaceDebrisHealth))]
    [RequireComponent(typeof(SpaceDebrisArmor))]
    public sealed class SpaceDebrisDamageReceiver : DamageReceiver
    {

    }
}