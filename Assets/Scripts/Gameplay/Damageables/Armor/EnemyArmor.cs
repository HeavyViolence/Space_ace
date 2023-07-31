using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Inventories;
using System;

namespace SpaceAce.Gameplay.Damageables
{
    public class EnemyArmor : Armor, IAmplifiable, IArmorDiffuserUser
    {
        private bool _armorDiffused = false;

        protected override void OnDisable()
        {
            base.OnDisable();

            _armorDiffused = false;
        }

        public void Amplify(float factor)
        {
            Value *= factor;
        }

        public bool Use(ArmorDiffuser diffuser)
        {
            if (diffuser is null) throw new ArgumentNullException(nameof(diffuser));

            if (_armorDiffused) return false;

            Value -= diffuser.ArmorReduction;
            _armorDiffused = true;

            return true;
        }
    }
}