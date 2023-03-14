using SpaceAce.Auxiliary;
using SpaceAce.Main.Audio;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class GunConfig : ScriptableObject
    {
        public const float MinFireDuration = 1f;
        public const float MaxFireDuration = 10f;

        public const float MinCooldown = 1f;
        public const float MaxCooldown = 10f;

        [SerializeField] private Sprite _gunIcon = null;

        [SerializeField] private float _fireDuration = MinFireDuration;
        [SerializeField] private float _fireDurationRandomDeviation = 0f;

        [SerializeField] private float _cooldown = MinCooldown;
        [SerializeField] private float _cooldownRandomDeviation = 0f;

        [SerializeField] private AudioCollection _fireAudio = null;

        public Sprite GunIcon => _gunIcon;
        public RangedFloat FireDuration { get; private set; }
        public RangedFloat Cooldown { get; private set; }
        public AudioCollection FireAudio => _fireAudio;

        private void OnEnable()
        {
            FireDuration = new(_fireDuration, _fireDurationRandomDeviation);
            Cooldown = new(_cooldown, _cooldownRandomDeviation);
        }
    }
}