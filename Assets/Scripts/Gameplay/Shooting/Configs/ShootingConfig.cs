using SpaceAce.Auxiliary;
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

        public const float MinNextWeaponsSwitchDelay = 0;
        public const float MaxNextWeaponsSwitchDelay = 60f;

        [SerializeField] private float _firstFireDelay = MinFirstFireDelay;
        [SerializeField] private float _firstFireDelayRandomDeviation = 0f;

        [SerializeField] private float _nextFireDelay = MinNextFireDelay;
        [SerializeField] private float _nextFireDelayRandomDeviation = 0f;

        [SerializeField] private bool _enableWeaponsSwitch = false;

        [SerializeField] private float _firstWeaponsSwitchDelay = MinFirstWeaponsSwitchDelay;
        [SerializeField] private float _firstWeaponsSwitchDelayRandomDeviation = 0f;

        [SerializeField] private float _nextWeaponsSwitchDelay = MinNextWeaponsSwitchDelay;
        [SerializeField] private float _nextWeaponsSwitchDelayRandomDeviation = 0f;

        public RangedFloat FirstFireDelay { get; private set; }
        public RangedFloat NextFireDelay { get; private set; }

        public bool WeaponsSwitchEnabled => _enableWeaponsSwitch;
        public RangedFloat FirstWeaponsSwitchDelay { get; private set; }
        public RangedFloat NextWeaponsSwitchDealy { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            FirstFireDelay = new(_firstFireDelay, _firstFireDelayRandomDeviation);
            NextFireDelay = new(_nextFireDelay, _nextFireDelayRandomDeviation);
            FirstWeaponsSwitchDelay = WeaponsSwitchEnabled ? new(_firstWeaponsSwitchDelay, _firstWeaponsSwitchDelayRandomDeviation) : RangedFloat.Zero;
            NextWeaponsSwitchDealy = WeaponsSwitchEnabled ? new(_nextWeaponsSwitchDelay, _nextWeaponsSwitchDelayRandomDeviation) : RangedFloat.Zero;
        }
    }
}