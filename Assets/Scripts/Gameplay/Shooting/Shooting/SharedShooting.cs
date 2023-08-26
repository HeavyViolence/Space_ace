using SpaceAce.Architecture;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Levels;
using SpaceAce.Main;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class SharedShooting : Shooting, IExperienceSource
    {
        protected static readonly GameServiceFastAccess<MasterCameraHolder> MasterCameraHolder = new();
        protected static readonly GameServiceFastAccess<GameModeLoader> GameModeLoader = new();
        protected static readonly GameServiceFastAccess<LevelCompleter> LevelCompleter = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();

        [SerializeField] private ShootingConfig _config;

        protected virtual float FirstFireDelay => _config.FirstFireDelay.RandomValue;
        protected virtual float NextFireDelay => _config.NextFireDelay.RandomValue;
        protected virtual float FirstWeaponsSwitchDelay => _config.FirstWeaponsSwitchDelay.RandomValue;
        protected virtual float NextWeaponsSwitchDelay => _config.NextWeaponsSwitchDealy.RandomValue;

        private Coroutine _firingRoutine;
        private Coroutine _weaponsSwitchRoutine;

        protected override void OnEnable()
        {
            base.OnEnable();

            StartShooting();

            GameModeLoader.Access.MainMenuLoadingStarted += (s, e) => StopShooting();
            LevelCompleter.Access.LevelConcluded += (s, e) => StopShooting();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            StopShooting();

            GameModeLoader.Access.MainMenuLoadingStarted -= (s, e) => StopShooting();
            LevelCompleter.Access.LevelConcluded -= (s, e) => StopShooting();
        }

        private void StartShooting()
        {
            if (_firingRoutine == null)
            {
                _firingRoutine = StartCoroutine(FireForever());

                if (WeaponGroupsAmount > 1) _weaponsSwitchRoutine = StartCoroutine(SwitchWeapons());
            }
        }

        private void StopShooting()
        {
            if (_firingRoutine != null)
            {
                StopCoroutine(_firingRoutine);
                _firingRoutine = null;

                if (WeaponGroupsAmount > 1)
                {
                    StopCoroutine(_weaponsSwitchRoutine);
                    _weaponsSwitchRoutine = null;
                }
            }
        }

        private IEnumerator FireForever()
        {
            while (MasterCameraHolder.Access.InsideViewport(transform.position) == false) yield return null;

            while (GamePauser.Access.Paused == true) yield return null;
            yield return new WaitForSeconds(FirstFireDelay);

            while (true)
            {
                var gun = NextActiveGun;

                while (gun.ReadyToFire == false) yield return null;

                gun.Fire();

                while (GamePauser.Access.Paused == true) yield return null;
                yield return new WaitForSeconds(NextFireDelay);
            }
        }

        private IEnumerator SwitchWeapons()
        {
            while (MasterCameraHolder.Access.InsideViewport(transform.position) == false) yield return null;

            while (GamePauser.Access.Paused == true) yield return null;
            yield return new WaitForSeconds(FirstWeaponsSwitchDelay);

            ActivateNextWeaponGroup(true);

            while (true)
            {
                while (GamePauser.Access.Paused == true) yield return null;
                yield return new WaitForSeconds(NextWeaponsSwitchDelay);

                ActivateNextWeaponGroup(true);
            }
        }

        public float GetExperience() => MaxDamagePerSecond;
    }
}