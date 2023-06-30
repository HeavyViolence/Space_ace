using SpaceAce.Architecture;
using SpaceAce.Gameplay.Damageables;
using System;
using UnityEngine;

namespace SpaceAce.UI
{
    public sealed class EntityView : MonoBehaviour
    {
        private static readonly GameServiceFastAccess<HUDDisplay> s_hudDisplay = new();

        public event EventHandler DisplayRequested;

        [SerializeField] private EntityViewConfig _config;

        private IDamageable _damageReceiver;

        public Sprite Icon => _config.Icon;
        public bool Active { get; private set; }
        public IHealthView Health {  get; private set; }
        public IArmorView Armor { get; private set; }
        public IWeaponView Weapons { get; private set; }

        private void Awake()
        {
            if (gameObject.TryGetComponent(out IDamageable damageReceiver) == true) _damageReceiver = damageReceiver;
            else throw new MissingComponentException($"Entity is missing a mandatory component of type {typeof(IDamageable)}!");

            if (gameObject.TryGetComponent(out IHealthView healthView) == true) Health = healthView;
            else throw new MissingComponentException($"Entity is missing a mandatory component of type {typeof(IHealthView)}!");

            if (gameObject.TryGetComponent(out IArmorView armorView) == true) Armor = armorView;
            else throw new MissingComponentException($"Entity is missing a mandatory component of type {typeof(IArmorView)}!");

            if (gameObject.TryGetComponent(out IWeaponView weaponView) == true) Weapons = weaponView;
            else if (_config.PlayerView == true) throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IWeaponView)}!");
        }

        private void OnEnable()
        {
            if (_config.PlayerView == true) s_hudDisplay.Access.RegisterPlayerView(this);
            else s_hudDisplay.Access.RegisterEntityView(this);

            _damageReceiver.DamageReceived += (s, e) => DisplayRequested?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        private void OnDisable()
        {
            if (_config.PlayerView == true) s_hudDisplay.Access.DeregisterPlayerView();
            else s_hudDisplay.Access.DeregisterEntityView(this);

            _damageReceiver.DamageReceived -= (s, e) => DisplayRequested?.Invoke(this, EventArgs.Empty);
            Active = false;
        }
    }
}