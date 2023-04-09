using SpaceAce.Gameplay.Experience;
using SpaceAce.Main.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class Shooting : MonoBehaviour, IExperienceSource
    {
        [SerializeField] private AudioCollection _weaponsSwitchAudio;

        private Dictionary<int, List<IGun>> _weaponGroups = new();
        private List<IGun> _activeWeaponGroup = null;

        private int _activeWeaponGroupIndex = -1;
        private int NextWeaponGroupIndex => ++_activeWeaponGroupIndex > WeaponGroupsAmount - 1 ? WeaponGroupsAmount - 1 : _activeWeaponGroupIndex;
        private int PreviousWeaponGroupIndex => --_activeWeaponGroupIndex < 0 ? 0 : _activeWeaponGroupIndex;

        private int _activeGunIndex = -1;
        private int NextGunIndex => ++_activeGunIndex % ActiveGunsAmount;

        protected IGun NextActiveGun => _activeWeaponGroup[NextGunIndex];
        protected int WeaponGroupsAmount => _weaponGroups.Count;
        protected int ActiveGunsAmount => _activeWeaponGroup.Count;

        private void Awake()
        {
            AssembleWeaponGroups();
        }

        protected virtual void OnEnable()
        {
            ActivateFirstWeaponGroup(false);
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
                    _weaponGroups.Add(gun.GunGroupID, new List<IGun>() { gun });
                }
            }
        }

        private bool ActivateWeaponGroup(int index, bool playAudioEffect)
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

            if (playAudioEffect)
            {
                _weaponsSwitchAudio.PlayRandomAudioClip(transform.position);
            }

            return true;
        }

        protected bool ActivateFirstWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(0, playAudioEffect);
        protected bool ActivateLastWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(WeaponGroupsAmount - 1, playAudioEffect);
        protected bool ActivateNextWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(NextWeaponGroupIndex, playAudioEffect);
        protected bool ActivatePreviousWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(PreviousWeaponGroupIndex, playAudioEffect);

        protected void StopActiveWeaponGroupShooting()
        {
            foreach (var gun in _activeWeaponGroup)
            {
                gun.StopFire();
            }
        }

        public float GetExperience()
        {
            float value = 0f;

            foreach (var activeGun in _activeWeaponGroup)
            {
                value += activeGun.MaxDamagePerSecond;
            }

            return value;
        }
    }
}