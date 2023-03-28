using SpaceAce.Main.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class Shooting : MonoBehaviour
    {
        [SerializeField] private AudioCollection _weaponsSwitchAudio;

        private Dictionary<int, List<IGun>> _weaponGroups = new();
        private List<IGun> _activeWeaponGroup = null;

        private int _activeWeaponGroupIndex = 0;
        private int NextWeaponGroupIndex => ++_activeWeaponGroupIndex > WeaponGroupsAmount - 1 ? WeaponGroupsAmount - 1 : _activeWeaponGroupIndex;
        private int PreviousWeaponGroupIndex => --_activeWeaponGroupIndex < 0 ? 0 : _activeWeaponGroupIndex;

        private int _activeGunIndex = -1;
        private int NextGunIndex => ++_activeGunIndex % ActiveGunsAmount;
        private bool _weaponsInitialized = false;

        protected IGun NextActiveGun => _activeWeaponGroup[NextGunIndex];
        protected int WeaponGroupsAmount => _weaponGroups.Count;
        protected int ActiveGunsAmount => _activeWeaponGroup.Count;

        private void Awake()
        {
            AssembleWeaponGroups();
        }

        protected virtual void OnEnable()
        {
            ActivateFirstWeaponGroup();
        }

        private void AssembleWeaponGroups()
        {
            foreach (var gun in gameObject.transform.root.GetComponentsInChildren<IGun>())
            {
                if (_weaponGroups.TryGetValue(gun.GunGroupID, out var guns))
                {
                    guns.Add(gun);
                }
                else
                {
                    List<IGun> newGunGroup = new() { gun };
                    _weaponGroups.Add(gun.GunGroupID, newGunGroup);
                }
            }
        }

        private bool ActivateWeaponGroup(int index)
        {
            if (index < 0 ||
                index > WeaponGroupsAmount - 1 ||
                index == _activeWeaponGroupIndex)
            {
                return false;
            }

            _activeWeaponGroup = _weaponGroups[index];
            _activeWeaponGroupIndex = index;
            _activeGunIndex = -1;

            if (_weaponsInitialized == false)
            {
                _weaponsInitialized = true;
            }
            else
            {
                _weaponsSwitchAudio.PlayRandomAudioClip(transform.position);
            }

            return true;
        }

        protected bool ActivateFirstWeaponGroup() => ActivateWeaponGroup(0);
        protected bool ActivateLastWeaponGroup() => ActivateWeaponGroup(WeaponGroupsAmount - 1);
        protected bool ActivateNextWeaponGroup() => ActivateWeaponGroup(NextWeaponGroupIndex);
        protected bool ActivatePreviousWeaponGroup() => ActivateWeaponGroup(PreviousWeaponGroupIndex);

        protected void StopActiveWeaponGroupShooting()
        {
            foreach (var gun in _activeWeaponGroup)
            {
                gun.StopFire();
            }
        }
    }
}