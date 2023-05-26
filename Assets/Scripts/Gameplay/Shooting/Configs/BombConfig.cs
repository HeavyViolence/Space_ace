using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Bomb config", menuName = "Space ace/Configs/Shooting/Bomb config")]
    public sealed class BombConfig : ScriptableObject
    {
        public const float MaxDamage = 10000f;
        public const float DefaultDamage = 500f;

        public const float MaxDamageDelay = 0.25f;
        public const float DefaultDamageDelay = 0.1f;

        [SerializeField] private float _damage = DefaultDamage;
        [SerializeField] private float _damageRandomDeviation = 0F;

        [SerializeField] private float _damageDelay = DefaultDamageDelay;
        [SerializeField] private float _damageDelayRandomDeviation = 0f;

        public RangedFloat Damage { get; private set; }
        public RangedFloat DamageDelay { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            Damage = new(_damage, _damageRandomDeviation);
            DamageDelay = new(_damageDelay, _damageDelayRandomDeviation);
        }
    }
}