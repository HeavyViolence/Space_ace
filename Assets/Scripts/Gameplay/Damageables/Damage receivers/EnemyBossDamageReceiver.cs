using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(BossHealth))]
    [RequireComponent(typeof(BossArmor))]
    public sealed class EnemyBossDamageReceiver : DamageReceiver
    {

    }
}