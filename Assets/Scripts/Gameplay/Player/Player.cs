using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Levels;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main.Saving;
using SpaceAce.UI;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    public sealed class Player : IGameService, ISavable, IFixedUpdatable, IUpdatable
    {
        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();

        public event EventHandler SavingRequested;
        public event EventHandler<PlayerShipSpawnedEventArgs> ShipSpawned;

        private readonly ObjectPoolEntryLookupTable _objectPoolEntryLookupTable;
        private readonly ObjectPoolEntry _defaultShip;
        private ObjectPoolEntry _selectedShip;

        private readonly GameControls _gameControls;
        private GameObject _activeShip;

        private IMovementController _shipMovementController;
        private IShootingController _shipShootingController;

        public string ID { get; }
        public string SaveName => "Player";
        public ObjectPoolEntry SelectedShip => _selectedShip != null ? _selectedShip : _defaultShip;
        public Inventory Inventory { get; }
        public Wallet Wallet { get; }
        public Experience Experience { get; }

        public Player(string id, ObjectPoolEntry defaultPlayerShip, ObjectPoolEntryLookupTable table)
        {
            if (StringID.IsValid(id) == false) throw new InvalidStringIDException();
            ID = id;

            if (defaultPlayerShip == null) throw new ArgumentNullException(nameof(defaultPlayerShip), "Attempted to pass an empty default player ship!");
            _defaultShip = defaultPlayerShip;

            if (table == null) throw new ArgumentNullException(nameof(table), $"Attempted to pass an empty {nameof(ObjectPoolEntryLookupTable)}!");
            _objectPoolEntryLookupTable = table;

            _gameControls = new();
            _gameControls.Gameplay.Disable();

            Inventory = new();
            Wallet = new();
            Experience = new();
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            _gameControls.Gameplay.Shooting.performed += (c) => _shipShootingController.Shoot();

            if (GameServices.TryGetService(out SavingSystem system) == true) system.Register(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));

            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded += LevelLoadedEventHandler;

                loader.MainMenuLoadingStarted += MainMenuLoadingStartedEventHandler;
                loader.MainMenuLoaded += MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelConcluded += LevelConcludedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out LevelRewardCollector collector) == true) collector.RewardDispensed += LevelRewardCollectedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelRewardCollector));

            if (GameServices.TryGetService(out HUDDisplay hudDisplay) == true)
            {
                hudDisplay.Enabled += HUDDisplayEnabledEventHandler;
                hudDisplay.Disabled += HUDDisplayDisabledEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
            }
        }

        public void OnUnsubscribe()
        {
            _gameControls.Gameplay.Shooting.performed -= (c) => _shipShootingController.Shoot();

            if (GameServices.TryGetService(out SavingSystem system) == true) system.Deregister(this);
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));

            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;

                loader.MainMenuLoadingStarted -= MainMenuLoadingStartedEventHandler;
                loader.MainMenuLoaded -= MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true) completer.LevelConcluded -= LevelConcludedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));

            if (GameServices.TryGetService(out LevelRewardCollector collector) == true) collector.RewardDispensed -= LevelRewardCollectedEventHandler;
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelRewardCollector));

            if (GameServices.TryGetService(out HUDDisplay hudDisplay) == true)
            {
                hudDisplay.Enabled -= HUDDisplayEnabledEventHandler;
                hudDisplay.Disabled -= HUDDisplayDisabledEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
            }
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public void OnUpdate()
        {
            WeaponsSwitchHandler();
        }

        public void OnFixedUpdate()
        {
            MoveShip();
        }

        public string GetState()
        {
            PlayerSavableData data = new(SelectedShip.AnchorName,
                                         Inventory.GetContent(),
                                         Wallet.Credits,
                                         Experience.Value);

            return JsonUtility.ToJson(data);
        }

        public void SetState(string state)
        {
            var data = JsonUtility.FromJson<PlayerSavableData>(state);

            if (_objectPoolEntryLookupTable.TryGetEntryByName(data.SelectedShipAnchorName, out var entry) == true) _selectedShip = entry;
            if (data.InventoryContent is not null) Inventory.AddItems(data.InventoryContent);
            if (data.Credits > 0) Wallet.AddCredits(data.Credits);
            if (data.Experience > 0f) Experience.Add(data.Experience);
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && ID.Equals(other.ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion

        private void MoveShip()
        {
            if (_gameControls.Gameplay.Movement.enabled && _shipMovementController is not null)
            {
                var movementDirection = _gameControls.Gameplay.Movement.ReadValue<Vector2>();
                _shipMovementController.Move(movementDirection);
            }
        }

        private void WeaponsSwitchHandler()
        {
            if (_gameControls.Gameplay.WeaponsSwitch.enabled && _shipShootingController is not null)
            {
                float value = _gameControls.Gameplay.WeaponsSwitch.ReadValue<float>();

                if (value > 0f) _shipShootingController.SwitchToNextWeapons();
                if (value < 0f) _shipShootingController.SwitchToPreviousWeapons();
            }
        }

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            if (_activeShip != null)
            {
                s_multiobjectPool.Access.ReleaseObject(SelectedShip.AnchorName, _activeShip, () => true);

                _activeShip = null;
                _shipMovementController = null;
                _shipShootingController = null;
            }

            SelectedShip.EnsureObjectPoolExistence();
            _activeShip = s_multiobjectPool.Access.GetObject(SelectedShip.AnchorName);

            if (_activeShip.TryGetComponent(out IMovementController movementController) == true) _shipMovementController = movementController;
            else throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IMovementController)}!");

            if (_activeShip.TryGetComponent(out IShootingController shootingController) == true) _shipShootingController = shootingController;
            else throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IShootingController)}!");

            if (_activeShip.TryGetComponent(out IDestroyable destroyable) == true)
            {
                ShipSpawned?.Invoke(this, new PlayerShipSpawnedEventArgs(destroyable));

                destroyable.Destroyed += (s, e) =>
                {
                    s_multiobjectPool.Access.ReleaseObject(SelectedShip.AnchorName, _activeShip, () => true);
                    _gameControls.Gameplay.Disable();
                };
            }
            else
            {
                throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IDestroyable)}!");
            }

            _gameControls.Gameplay.Enable();
        }

        private void MainMenuLoadingStartedEventHandler(object sender, LoadingStartedEventArgs e)
        {
            _gameControls.Gameplay.Movement.Disable();
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            if (_activeShip != null)
            {
                s_multiobjectPool.Access.ReleaseObject(SelectedShip.AnchorName, _activeShip, () => true);

                _activeShip = null;
                _shipMovementController = null;
                _shipShootingController = null;
            }
        }

        private void LevelConcludedEventHandler(object sender, EventArgs e)
        {
            _gameControls.Gameplay.Shooting.Disable();
            _shipShootingController.StopShooting();
        }

        private void LevelRewardCollectedEventHandler(object sender, LevelRewardDispensedEventArgs e)
        {
            Wallet.AddCredits(e.Credits);
            Experience.Add(e.Experience);
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }

        private void HUDDisplayEnabledEventHandler(object sender, EventArgs e)
        {
            if (s_gameModeLoader.Access.GameState == GameState.Level) _gameControls.Gameplay.Enable();

            if (s_gameModeLoader.Access.GameState == GameState.LevelPassed ||
                s_gameModeLoader.Access.GameState == GameState.LevelFailed)
            {
                _gameControls.Gameplay.Movement.Enable();
            }
        }

        private void HUDDisplayDisabledEventHandler(object sender, EventArgs e)
        {
            _gameControls.Gameplay.Disable();
            _shipShootingController.StopShooting();
        }

        #endregion

        public void SetSelectedPlayerShip(ObjectPoolEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry), "Attempted to pass an empty selected ship!");

            _selectedShip = entry;
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}