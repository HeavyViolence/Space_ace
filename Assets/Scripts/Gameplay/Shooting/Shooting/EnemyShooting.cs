using SpaceAce.Architecture;
using SpaceAce.Levels;
using SpaceAce.Main;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyShooting : Shooting
    {
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();
        private static readonly GameServiceFastAccess<LevelCompleter> s_levelCompleter = new();

        [SerializeField] private ShootingConfig _config;

        private float _weaponsSwitchTimer;
        private float _nextWeaponsSwitchDelay;

        Coroutine _firingRoutine;

        protected sealed override void OnEnable()
        {
            base.OnEnable();

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

        private void Update()
        {
            if (s_masterCameraHolder.Access.InsideViewport(transform.position) == true)
            {
                if (_weaponsSwitchTimer < _nextWeaponsSwitchDelay)
                {
                    _weaponsSwitchTimer += Time.deltaTime;
                }
                else
                {
                    _weaponsSwitchTimer = 0f;
                    _nextWeaponsSwitchDelay = _config.NextWeaponsSwitchDealy.RandomValue;

                    ActivateNextWeaponGroup(true);
                }
            }
        }

        private void StartShooting()
        {
            if (_firingRoutine == null)
            {
                _firingRoutine = StartCoroutine(FireForever());
                _weaponsSwitchTimer = 0f;
                _nextWeaponsSwitchDelay = _config.FirstWeaponsSwitchDelay.RandomValue;
            }
        }

        private IEnumerator FireForever()
        {
            while (s_masterCameraHolder.Access.InsideViewport(transform.position) == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_config.FirstFireDelay.RandomValue);

            while (true)
            {
                var gun = NextActiveGun;

                while (gun.ReadyToFire == false)
                {
                    yield return null;
                }

                gun.Fire();

                yield return new WaitForSeconds(_config.NextFireDelay.RandomValue);
            }
        }

        private void StopShooting()
        {
            if (_firingRoutine != null)
            {
                StopCoroutine(_firingRoutine);
                _firingRoutine = null;
            }
        }
    }
}