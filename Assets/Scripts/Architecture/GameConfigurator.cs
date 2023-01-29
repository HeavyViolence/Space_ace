using SpaceAce.Auxiliary;
using SpaceAce.Main;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce
{
    namespace Architecture
    {
        public sealed class GameConfigurator : MonoBehaviour
        {
            #region constants

            #endregion

            #region dependency injection

            [SerializeField] private int _idGeneratorSeed;

            [SerializeField] private GameObject _spaceBackgroundPrefab;
            [SerializeField] private List<Material> _spaceBackgroundMaterials;

            #endregion

            #region private fields

            private readonly List<object> _gameServices = new();
            private StringID _idGenerator;

            #endregion

            #region game setup

            private void Awake()
            {
                CreateGameServices();
                InitializeGameServices();
                PerformEventsSubscriptionForGameServices();
            }

            private void Update()
            {
                UpdateGameServices();
            }

            private void OnDestroy()
            {
                PerformEventsUnsubscriptionForGameServices();
                ClearGameServices();
            }

            private void CreateGameServices()
            {
                _idGenerator = new(_idGeneratorSeed);

                MasterCameraHolder cameraHolder = new();
                MasterAudioListenerHolder audioListenerHolder = new(cameraHolder.MasterCameraAnchor);

                _gameServices.Add(cameraHolder);
                _gameServices.Add(audioListenerHolder);
                _gameServices.Add(new SpaceBackground(_spaceBackgroundPrefab, _spaceBackgroundMaterials));
            }

            private void InitializeGameServices()
            {
                foreach (var service in _gameServices)
                {
                    if (service is IInitializable value)
                    {
                        value.OnInitialize();
                    }
                }
            }

            private void PerformEventsSubscriptionForGameServices()
            {
                foreach (var service in _gameServices)
                {
                    if (service is IInitializable value)
                    {
                        value.OnSubscribe();
                    }
                }
            }

            private void PerformEventsUnsubscriptionForGameServices()
            {
                foreach (var service in _gameServices)
                {
                    if (service is IInitializable value)
                    {
                        value.OnUnsubscribe();
                    }
                }
            }

            private void ClearGameServices()
            {
                foreach (var service in _gameServices)
                {
                    if (service is IInitializable value)
                    {
                        value.OnClear();
                    }
                }

                _gameServices.Clear();
            }

            private void UpdateGameServices()
            {
                foreach (var service in _gameServices)
                {
                    if (service is IUpdatable value)
                    {
                        value.OnUpdate();
                    }
                }
            }

            #endregion
        }
    }
}