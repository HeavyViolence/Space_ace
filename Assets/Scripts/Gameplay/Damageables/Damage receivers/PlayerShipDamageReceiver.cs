using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(PlayerShipHealth))]
    [RequireComponent(typeof(PlayerShipArmor))]
    public sealed class PlayerShipDamageReceiver : DamageReceiver
    {

    }
}