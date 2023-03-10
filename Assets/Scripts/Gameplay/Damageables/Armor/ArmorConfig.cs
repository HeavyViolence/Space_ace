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

        [SerializeField] private float _armorValue = MinArmor;
        [SerializeField] private float _armorValueRandomDeviation = 0f;

        public bool ArmorEnabled => _armorEnabled;

        public float RandomArmorValue => ArmorEnabled ? _armorValue + _armorValueRandomDeviation * AuxMath.RandomNormal : MinArmor;
        public float MinArmorValue => ArmorEnabled ? _armorValue - _armorValueRandomDeviation : MinArmor;
        public float MaxArmorvalue => ArmorEnabled ? _armorValue + _armorValueRandomDeviation : MinArmor;
    }
}