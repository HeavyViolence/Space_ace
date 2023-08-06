namespace SpaceAce.Gameplay.Shooting
{
    public sealed class BossProjectileGun : ProjectileGun
    {
        protected override float GetNextProjectileDamage(string hitID) => NextProjectileDamage;
    }
}