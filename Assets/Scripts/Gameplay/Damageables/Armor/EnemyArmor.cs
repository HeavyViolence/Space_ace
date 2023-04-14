using SpaceAce.Gameplay.Amplifications;

namespace SpaceAce.Gameplay.Damageables
{
    public class EnemyArmor : Armor, IAmplifiable
    {
        public void Amplify(float factor)
        {
            Value *= factor;
        }
    }
}