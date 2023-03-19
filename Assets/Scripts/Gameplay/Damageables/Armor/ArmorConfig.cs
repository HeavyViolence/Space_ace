using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [CreateAssetMenu(fileName = "Armor config", menuName = "Space ace/Configs/Damageables/Armor config")]
    public sealed class ArmorConfig : ScriptableObject
    {
        public const float MinArmor = 0f;
        public const float MaxArmor = 1000f;

        [SerializeField] private bool _armorEnabled = false;

        [SerializeField] private float _armor = MinArmor;
        [SerializeField] private float _armorRandomDeviation = 0f;

        public bool ArmorEnabled => _armorEnabled;
        public RangedFloat Armor { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            Armor = ArmorEnabled ? new(_armor, _armorRandomDeviation) : RangedFloat.Zero;
        }
    }
}