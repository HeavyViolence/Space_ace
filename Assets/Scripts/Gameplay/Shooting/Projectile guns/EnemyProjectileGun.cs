using SpaceAce.Gameplay.Amplifications;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyProjectileGun : ProjectileGun, IAmplifiable
    {
        private float _damageAmplifier = 1f;

        public override float MaxDamagePerSecond => base.MaxDamagePerSecond * _damageAmplifier;
        protected override float NextProjectileDamage => base.NextProjectileDamage * _damageAmplifier;

        private void OnEnable()
        {
            _damageAmplifier = 1f;
        }

        public void Amplify(float factor)
        {
            _damageAmplifier *= factor;
        }
    }
}