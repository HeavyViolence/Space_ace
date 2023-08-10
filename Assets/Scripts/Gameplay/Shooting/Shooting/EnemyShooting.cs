using SpaceAce.Gameplay.Amplifications;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyShooting : SharedShooting, IAmplifiable
    {
        protected override float FirstFireDelay => base.FirstFireDelay / _amplificationFactor;
        protected override float NextFireDelay => base.NextFireDelay / _amplificationFactor;
        protected override float FirstWeaponsSwitchDelay => base.FirstWeaponsSwitchDelay / _amplificationFactor;
        protected override float NextWeaponsSwitchDelay => base.NextWeaponsSwitchDelay / _amplificationFactor;

        private float _amplificationFactor = 1f;

        protected sealed override void OnDisable()
        {
            base.OnDisable();

            _amplificationFactor = 1f;
        }

        public void Amplify(float factor) => _amplificationFactor = factor;
    }
}