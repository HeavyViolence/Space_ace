using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(MeteorHealth))]
    [RequireComponent(typeof(MeteorArmor))]
    public sealed class MeteorDamageReceiver : DamageReceiver
    {

    }
}