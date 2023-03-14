using SpaceAce.Auxiliary;
using SpaceAce.Main.Audio;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Shooting config", menuName = "Space ace/Configs/Shooting/Shooting config")]
    public sealed class ShootingConfig : ScriptableObject
    {
        public const float MinFirstFireDelay = 0f;
        public const float MaxFirstFireDelay = 10f;

        public const float MinNextFireDelay = 0f;
        public const float MaxNextFireDelay = 10f;

        public const float MinFirstWeaponsSwitchDelay = 0f;
        public const float MaxFirstWeaponsSwitchDelay = 60f;

        public const float MinNextWeaponsSwitchDelay = 0f;
        public const float MaxNextWeaponsSwitchDelay = 60f;

        [SerializeField] private float _firstFireDelay = MinFirstFireDelay;
        [SerializeField] private float _firstFireDelayRandomDeviation = 0f;

        [SerializeField] private float _nextFireDelay = MinNextFireDelay;
        [SerializeField] private float _nextFireDelayRandomDeviation = 0f;

        [SerializeField] private bool _weaponsSwitchEnabled = false;

        [SerializeField] private float _firstWeaponsSwitchDelay = MinFirstWeaponsSwitchDelay;
        [SerializeField] private float _firstWeaponsSwitchDelayRandomDeviation = 0f;

        [SerializeField] private float _nextWeaponsSwitchDelay = MinNextWeaponsSwitchDelay;
        [SerializeField] private float _nextWeaponsSwitchDelayRandomDeviation = 0f;

        [SerializeField] private AudioCollection _weaponsSwitchAudio;

        public RangedFloat FirstFireDelay { get; private set; }
        public RangedFloat NextFireDelay { get; private set; }

        public bool WeaponsSwitchEnabled => _weaponsSwitchEnabled;
        public RangedFloat FirstWeaponsSwitchDelay { get; private set; }
        public RangedFloat NextWeaponsSwitchDelay { get; private set; }
        public AudioCollection WeaponsSwitchAudio => WeaponsSwitchEnabled ? _weaponsSwitchAudio : null;

        private void OnEnable()
        {
            FirstFireDelay = new(_firstFireDelay, _firstFireDelayRandomDeviation);
            NextFireDelay = new(_nextFireDelay, _nextFireDelayRandomDeviation);
            FirstWeaponsSwitchDelay = new(_firstWeaponsSwitchDelay, _firstWeaponsSwitchDelayRandomDeviation);
            NextWeaponsSwitchDelay = new(_nextWeaponsSwitchDelay, _nextWeaponsSwitchDelayRandomDeviation);
        }
    }
}