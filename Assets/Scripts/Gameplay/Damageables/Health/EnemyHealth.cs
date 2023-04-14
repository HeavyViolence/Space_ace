using SpaceAce.Gameplay.Amplifications;

namespace SpaceAce.Gameplay.Damageables
{
    public class EnemyHealth : Health, IAmplifiable
    {
        public void Amplify(float factor)
        {
            MaxValue *= factor;
            RegenPerSecond *= factor;
        }
    }
}