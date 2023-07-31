using SpaceAce.Gameplay.Amplifications;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyProjectileGun : ProjectileGun, IAmplifiable
    {
        private float _amplificationFactor = 1f;

        public override float MaxDamagePerSecond => base.MaxDamagePerSecond * _amplificationFactor;

        protected override float NextProjectileTopSpeed => base.NextProjectileTopSpeed * _amplificationFactor;
        protected override float NextProjectileTopSpeedGainDuration => base.NextProjectileTopSpeedGainDuration / _amplificationFactor;
        protected override float NextProjectileDamage => base.NextProjectileDamage * _amplificationFactor;
        protected override float NextCooldown => base.NextCooldown / _amplificationFactor;
        protected override float NextFireDuration => base.NextFireDuration * _amplificationFactor;
        protected override float NextFireRate => base.NextFireRate * _amplificationFactor;

        protected override void OnEnable()
        {
            base.OnEnable();

            _amplificationFactor = 1f;
        }

        public void Amplify(float factor)
        {
            _amplificationFactor = factor;
        }
    }
}