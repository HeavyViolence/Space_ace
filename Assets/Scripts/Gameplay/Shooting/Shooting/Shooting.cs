using SpaceAce.Gameplay.Experience;
using SpaceAce.Main.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class Shooting : MonoBehaviour, IExperienceSource
    {
        [SerializeField] private AudioCollection _weaponsSwitchAudio;

        private Dictionary<int, List<IGun>> _availableWeaponGroups = new();
        private List<IGun> _activeWeaponGroup = null;

        private int _activeWeaponGroupIndex = -1;
        private int _activeGunIndex = -1;

        protected IGun NextActiveGun => _activeWeaponGroup[++_activeGunIndex % ActiveGunsAmount];
        protected int WeaponGroupsAmount => _availableWeaponGroups.Count;
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
                if (_availableWeaponGroups.TryGetValue(gun.GroupID, out var guns))
                {
                    guns.Add(gun);
                }
                else
                {
                    _availableWeaponGroups.Add(gun.GroupID, new List<IGun>() { gun });
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

            _activeWeaponGroup = _availableWeaponGroups[index];
            _activeGunIndex = -1;

            if (playAudioEffect)
            {
                _weaponsSwitchAudio.PlayRandomAudioClip(transform.position);
            }

            return true;
        }

        protected bool ActivateFirstWeaponGroup(bool playAudioEffect)
        {
            if (ActivateWeaponGroup(0, playAudioEffect) == true)
            {
                _activeWeaponGroupIndex = 0;

                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool ActivateLastWeaponGroup(bool playAudioEffect)
        {
            if (ActivateWeaponGroup(WeaponGroupsAmount - 1, playAudioEffect) == true)
            {
                _activeWeaponGroupIndex = WeaponGroupsAmount - 1;

                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool ActivateNextWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(++_activeWeaponGroupIndex % WeaponGroupsAmount, playAudioEffect);
        protected bool ActivatePreviousWeaponGroup(bool playAudioEffect) => ActivateWeaponGroup(--_activeWeaponGroupIndex < 0 ? WeaponGroupsAmount - 1
                                                                                                                              : _activeWeaponGroupIndex,
                                                                                                playAudioEffect);

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