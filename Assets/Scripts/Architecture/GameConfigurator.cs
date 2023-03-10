using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Players;
using SpaceAce.Levels;
using SpaceAce.Main;
using SpaceAce.Main.Audio;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main.Saving;
using SpaceAce.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SpaceAce.Architecture
{
    public sealed class GameConfigurator : MonoBehaviour
    {
        #region constants

        public const float MinWidthDelta = 0.05f;
        public const float MaxWidthDelta = 0.1f;
        public const float DefaultWidthDelta = 0.075f;

        #endregion

        #region dependency injection

        [SerializeField] private int _idGeneratorSeed;

        [SerializeField] private float _cameraSize = MasterCameraHolder.MinCameraSize;

        [SerializeField] private float _spaceBackgroundWidthDelta = DefaultWidthDelta;
        [SerializeField] private Material _mainMenuSpaceBackground;
        [SerializeField] private List<Material> _spaceBackgrounds;
        [SerializeField] private GameObject _dustfieldPrefab;

        [SerializeField] private List<LevelConfig> _levelConfigs;

        [SerializeField] private AnimationCurve _fadingCurve;
        [SerializeField] private Color32 _fadingColor;

        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioCollection _music;

        [SerializeField] private UIAssets _uiContainer;

        [SerializeField] private ObjectPoolEntry _defaultPlayerShip;
        [SerializeField] private ObjectPoolEntryLookupTable _objectPoolEntryLookupTable;

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

        private void Start()
        {
            RunGameServices();
        }

        private void Update()
        {
            UpdateGameServices();
        }

        private void FixedUpdate()
        {
            FixedUpdateGameServices();
        }

        private void OnDestroy()
        {
            PerformEventsUnsubscriptionForGameServices();
            ClearGameServices();
        }

        private void CreateGameServices()
        {
            _idGenerator = new(_idGeneratorSeed);

            MasterCameraHolder cameraHolder = new(_cameraSize);
            MasterAudioListenerHolder audioListenerHolder = new(cameraHolder.MasterCameraAnchor);

            SpaceBackground background = new(cameraHolder.ViewportLowerLeftCornerWorldPosition,
                                             cameraHolder.ViewportLowerRightCornerWorldPosition,
                                             cameraHolder.MasterCamera.aspect,
                                             _spaceBackgroundWidthDelta,
                                             _mainMenuSpaceBackground,
                                             _spaceBackgrounds,
                                             _dustfieldPrefab);

            _gameServices.Add(cameraHolder);
            _gameServices.Add(audioListenerHolder);
            _gameServices.Add(background);
            _gameServices.Add(new GameModeLoader(_levelConfigs));
            _gameServices.Add(new ScreenFader(_fadingCurve, _fadingColor));
            _gameServices.Add(new MultiobjectPool());
            _gameServices.Add(new LevelCompleter());
            _gameServices.Add(new GamePauser());
            _gameServices.Add(new UIDisplay(_uiContainer));

            _gameServices.Add(new SavingSystem(_idGenerator.Next()));
            _gameServices.Add(new CameraShaker(_idGenerator.Next(), cameraHolder.MasterCameraAnchor));
            _gameServices.Add(new AudioPlayer(_idGenerator.Next(), _audioMixer));
            _gameServices.Add(new MusicPlayer(_idGenerator.Next(), _music));
            _gameServices.Add(new LevelUnlocker(_idGenerator.Next()));
            _gameServices.Add(new BestLevelsRunsStatisticsCollector(_idGenerator.Next()));
            _gameServices.Add(new Player(_idGenerator.Next(), _defaultPlayerShip, _objectPoolEntryLookupTable));
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

        private void FixedUpdateGameServices()
        {
            foreach (var service in _gameServices)
            {
                if (service is IFixedUpdatable value)
                {
                    value.OnFixedUpdate();
                }
            }
        }

        private void RunGameServices()
        {
            foreach (var service in _gameServices)
            {
                if (service is IRunnable value)
                {
                    value.OnRun();
                }
            }
        }

        #endregion
    }
}