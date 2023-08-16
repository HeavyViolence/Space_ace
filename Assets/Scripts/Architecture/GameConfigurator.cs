using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Players;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Levels;
using SpaceAce.Main;
using SpaceAce.Main.Audio;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main.Saving;
using SpaceAce.UI;
using SpaceAce.Visualization;
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
        [SerializeField] private AudioCollection _bossSpawnAlarm;

        [SerializeField] private UIAssets _uiAssets;

        [SerializeField] private ObjectPoolEntry _defaultPlayerShip;
        [SerializeField] private ObjectPoolEntryLookupTable _objectPoolEntryLookupTable;

        [SerializeField] private InventoryItemIconsConfig _itemIconsConfig;
        [SerializeField] private InventoryItemRarityColorsConfig _itemRarityColorsConfig;
        [SerializeField] private ObjectPoolEntry _lootItemBox;

        [SerializeField] private AnimationCurve _easingCurveIn;
        [SerializeField] private AnimationCurve _easingCurveOut;
        [SerializeField] private AnimationCurve _easingCurveInOut;

        #endregion

        #region private fields

        private readonly List<object> _gameServices = new();

        private List<IUpdatable> _updatableGameServices = null;
        private List<IFixedUpdatable> _fixedUpdatableGameServices = null;

        private StringID _idGenerator;

        #endregion

        #region game setup

        private void Awake()
        {
            CreateGameServices();
            InitializeGameServices();
            PerformEventsSubscriptionForGameServices();

            AuxMath.EasingCurveIn = _easingCurveIn;
            AuxMath.EasingCurveOut = _easingCurveOut;
            AuxMath.EasingCurveInOut = _easingCurveInOut;
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
            _gameServices.Add(new LevelRewardCollector());
            _gameServices.Add(new LevelTimer());
            _gameServices.Add(new GamePauser());
            _gameServices.Add(new EnemySpawner());
            _gameServices.Add(new BossSpawner(_bossSpawnAlarm));
            _gameServices.Add(new MeteorSpawner());
            _gameServices.Add(new SpaceDebrisSpawner());
            _gameServices.Add(new BombSpawner());
            _gameServices.Add(new EntityVisualizer(_itemIconsConfig, _itemRarityColorsConfig));
            _gameServices.Add(new LootSpawner(_lootItemBox));
            _gameServices.Add(new SpecialEffectsMediator());

            _gameServices.Add(new SavingSystem());
            _gameServices.Add(new CameraShaker(_idGenerator.Next(), cameraHolder.MasterCameraAnchor));
            _gameServices.Add(new AudioPlayer(_idGenerator.Next(), _audioMixer));
            _gameServices.Add(new MusicPlayer(_idGenerator.Next(), _music));
            _gameServices.Add(new LevelUnlocker(_idGenerator.Next()));
            _gameServices.Add(new BestLevelsRunsStatisticsCollector(_idGenerator.Next()));
            _gameServices.Add(new Player(_idGenerator.Next(), _defaultPlayerShip, _objectPoolEntryLookupTable));

            _gameServices.Add(new MainMenuDisplay(_uiAssets));
            _gameServices.Add(new LevelSelectionDisplay(_uiAssets));
            _gameServices.Add(new InventoryDisplay(_uiAssets));
            _gameServices.Add(new HUDDisplay(_uiAssets));
            _gameServices.Add(new PauseDisplay(_uiAssets));
        }

        private void InitializeGameServices()
        {
            foreach (var service in _gameServices) if (service is IGameService value) value.OnInitialize();
        }

        private void PerformEventsSubscriptionForGameServices()
        {
            foreach (var service in _gameServices) if (service is IGameService value) value.OnSubscribe();
        }

        private void PerformEventsUnsubscriptionForGameServices()
        {
            foreach (var service in _gameServices) if (service is IGameService value) value.OnUnsubscribe();
        }

        private void ClearGameServices()
        {
            foreach (var service in _gameServices) if (service is IGameService value) value.OnClear();

            _gameServices.Clear();
        }

        private void UpdateGameServices()
        {
            if (_updatableGameServices is null)
            {
                _updatableGameServices = new();

                foreach (var service in _gameServices) if (service is IUpdatable value) _updatableGameServices.Add(value);
            }

            foreach (var service in _updatableGameServices) service.OnUpdate();
        }

        private void FixedUpdateGameServices()
        {
            if (_fixedUpdatableGameServices is null)
            {
                _fixedUpdatableGameServices = new();

                foreach (var service in _gameServices) if (service is IFixedUpdatable value) _fixedUpdatableGameServices.Add(value);
            }

            foreach (var service in _fixedUpdatableGameServices) service.OnFixedUpdate();
        }

        private void RunGameServices()
        {
            foreach (var service in _gameServices) if (service is IRunnable value) value.OnRun();
        }

        #endregion
    }
}