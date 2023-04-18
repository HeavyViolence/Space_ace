using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(EnemyArmor))]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyDamageReceiver : DamageReceiver
    {

    }
}