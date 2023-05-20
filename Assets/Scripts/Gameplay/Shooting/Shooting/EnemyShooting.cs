using SpaceAce.Architecture;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Levels;
using SpaceAce.Main;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyShooting : Shooting, IAmplifiable
    {
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();
        private static readonly GameServiceFastAccess<LevelCompleter> s_levelCompleter = new();

        [SerializeField] private ShootingConfig _config;

        private float FirstFireDelay => _config.FirstFireDelay.RandomValue / _amplificationFactor;
        private float NextFireDelay => _config.NextFireDelay.RandomValue / _amplificationFactor;
        private float FirstWeaponsSwitchDelay => _config.FirstWeaponsSwitchDelay.RandomValue / _amplificationFactor;
        private float NextWeaponsSwitchDelay => _config.NextWeaponsSwitchDealy.RandomValue / _amplificationFactor;

        private Coroutine _firingRoutine;
        private Coroutine _weaponsSwitchRoutine;

        private float _amplificationFactor = 1f;

        protected sealed override void OnEnable()
        {
            base.OnEnable();

            _amplificationFactor = 1f;

            StartShooting();

            s_gameModeLoader.Access.MainMenuLoadingStarted += (s, e) => StopShooting();
            s_levelCompleter.Access.LevelConcluded += (s, e) => StopShooting();
        }

        private void OnDisable()
        {
            StopShooting();

            s_gameModeLoader.Access.MainMenuLoadingStarted -= (s, e) => StopShooting();
            s_levelCompleter.Access.LevelConcluded -= (s, e) => StopShooting();
        }

        private void StartShooting()
        {
            if (_firingRoutine == null)
            {
                _firingRoutine = StartCoroutine(FireForever());

                if (WeaponGroupsAmount > 1)
                {
                    _weaponsSwitchRoutine = StartCoroutine(SwitchWeapons());
                }
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
            while (s_masterCameraHolder.Access.InsideViewport(transform.position) == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(FirstFireDelay);

            while (true)
            {
                var gun = NextActiveGun;

                while (gun.ReadyToFire == false)
                {
                    yield return null;
                }

                gun.Fire();

                yield return new WaitForSeconds(NextFireDelay);
            }
        }

        private IEnumerator SwitchWeapons()
        {
            while (s_masterCameraHolder.Access.InsideViewport(transform.position) == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(FirstWeaponsSwitchDelay);

            ActivateNextWeaponGroup(true);

            while (true)
            {
                yield return new WaitForSeconds(NextWeaponsSwitchDelay);

                ActivateNextWeaponGroup(true);
            }
        }

        public void Amplify(float factor)
        {
            _amplificationFactor = factor;
        }
    }
}