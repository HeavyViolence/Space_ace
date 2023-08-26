using SpaceAce.Main.Audio;
using SpaceAce.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class Shooting : MonoBehaviour, IWeaponView, IGunner
    {
        public event EventHandler GunFired, TargetHit;

        [SerializeField] private AudioCollection _weaponsSwitchAudio;

        private readonly Dictionary<int, List<IGun>> _availableWeaponGroups = new();
        private List<IGun> _activeWeaponGroup = null;

        private int _activeGunIndex = -1;

        protected IGun NextActiveGun => _activeWeaponGroup[++_activeGunIndex % ActiveGunsAmount];
        protected int ActiveGunsAmount => _activeWeaponGroup.Count;

        public int ActiveWeaponGroupIndex { get; private set; } = -1;
        public int WeaponGroupsAmount => _availableWeaponGroups.Count;

        public float MaxDamagePerSecond
        {
            get
            {
                float value = 0f;

                foreach (var activeGun in _activeWeaponGroup) value += activeGun.MaxDamagePerSecond;

                return value;
            }
        }

        protected virtual void Awake()
        {
            AssembleWeaponGroups();
        }

        protected virtual void OnEnable()
        {
            ActivateFirstWeaponGroup(false);

            foreach (var group in _availableWeaponGroups)
            {
                foreach (var gun in group.Value)
                {
                    gun.ShotFired += (sender, args) => GunFired?.Invoke(this, EventArgs.Empty);
                    gun.Hit += (sender, args) => TargetHit?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var group in _availableWeaponGroups)
            {
                foreach (var gun in group.Value)
                {
                    gun.ShotFired -= (sender, args) => GunFired?.Invoke(this, EventArgs.Empty);
                    gun.Hit -= (sender, args) => TargetHit?.Invoke(this, EventArgs.Empty);
                }
            }

            GunFired = null;
            TargetHit = null;
        }

        private void AssembleWeaponGroups()
        {
            foreach (var gun in gameObject.transform.root.GetComponentsInChildren<IGun>())
            {
                if (_availableWeaponGroups.TryGetValue(gun.GroupID, out var guns) == true) guns.Add(gun);
                else _availableWeaponGroups.Add(gun.GroupID, new List<IGun>() { gun });
            }
        }

        private bool ActivateWeaponGroup(int index, bool playAudioEffect)
        {
            if (index < 0 ||
                index > WeaponGroupsAmount - 1 ||
                index == ActiveWeaponGroupIndex) return false;

            _activeWeaponGroup = _availableWeaponGroups[index];
            ActiveWeaponGroupIndex = index;
            _activeGunIndex = -1;

            if (playAudioEffect) _weaponsSwitchAudio.PlayRandomAudioClip(transform.position);

            return true;
        }

        protected bool ActivateFirstWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(0, playAudioEffect);

        protected bool ActivateLastWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(_availableWeaponGroups.Count - 1, playAudioEffect);

        protected bool ActivateNextWeaponGroup(bool playAudioEffect) =>
            ActivateWeaponGroup(++ActiveWeaponGroupIndex % WeaponGroupsAmount, playAudioEffect);

        protected bool ActivatePreviousWeaponGroup(bool playAudioEffect) =>
            ActivateWeaponGroup(--ActiveWeaponGroupIndex < 0 ? WeaponGroupsAmount - 1
                                                             : ActiveWeaponGroupIndex, playAudioEffect);

        protected void StopActiveWeaponGroupShooting()
        {
            foreach (var gun in _activeWeaponGroup) gun.StopFire();
        }
    }
}