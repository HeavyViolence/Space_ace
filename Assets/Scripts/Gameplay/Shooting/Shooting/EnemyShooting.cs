using SpaceAce.Architecture;
using SpaceAce.Main;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public sealed class EnemyShooting : Shooting
    {
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();

        [SerializeField] private ShootingConfig _config;

        private float _weaponsSwitchTimer;
        private float _nextWeaponsSwitchDelay;

        Coroutine _firingRoutine;

        protected sealed override void OnEnable()
        {
            base.OnEnable();

            _firingRoutine = StartCoroutine(FireForever());
            _weaponsSwitchTimer = 0f;
            _nextWeaponsSwitchDelay = _config.FirstWeaponsSwitchDelay.RandomValue;
        }

        private void OnDisable()
        {
            StopCoroutine(_firingRoutine);
            _firingRoutine = null;
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

        private IEnumerator FireForever()
        {
            while (s_masterCameraHolder.Access.InsideViewport(transform.position) == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_config.FirstFireDelay.RandomValue);

            while (true)
            {
                var gunToFire = NextActiveGun;

                while (gunToFire.ReadyToFire == false)
                {
                    yield return null;
                }

                gunToFire.Fire();

                yield return new WaitForSeconds(_config.NextFireDelay.RandomValue);
            }
        }
    }
}