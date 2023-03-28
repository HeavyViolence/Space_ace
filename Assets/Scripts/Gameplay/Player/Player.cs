using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Levels;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main.Saving;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    public sealed class Player : IInitializable, ISavable, IFixedUpdatable, IUpdatable
    {
        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();

        public event EventHandler SavingRequested;

        private ObjectPoolEntryLookupTable _objectPoolEntryLookupTable;
        private GameControls _gameControls;
        private GameObject _activeShip;

        private IMovementController _shipMovementController;
        private IShootingController _shipShootingController;

        public string ID { get; }
        public string SaveName => "Player";
        public ObjectPoolEntry SelectedShip { get; private set; }

        public Player(string id, ObjectPoolEntry defaultPlayerShip, ObjectPoolEntryLookupTable table)
        {
            if (StringID.IsValid(id) == false)
            {
                throw new InvalidStringIDException();
            }

            ID = id;

            if (defaultPlayerShip == null)
            {
                throw new ArgumentNullException(nameof(defaultPlayerShip), "Attempted to pass an empty default player ship!");
            }

            SelectedShip = defaultPlayerShip;

            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), $"Attempted to pass an empty {nameof(ObjectPoolEntryLookupTable)}!");
            }

            _objectPoolEntryLookupTable = table;

            _gameControls = new();
            _gameControls.Gameplay.Disable();
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            _gameControls.Gameplay.Shooting.performed += (c) => _shipShootingController.Shoot();

            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Register(this);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));
            }

            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded += LevelLoadedEventHandler;
                loader.MainMenuLoaded += MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelConcluded += LevelConcludedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
            }
        }

        public void OnUnsubscribe()
        {
            _gameControls.Gameplay.Shooting.performed -= (c) => _shipShootingController.Shoot();

            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Deregister(this);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(SavingSystem));
            }

            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;
                loader.MainMenuLoaded -= MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }

            if (GameServices.TryGetService(out LevelCompleter completer) == true)
            {
                completer.LevelConcluded -= LevelConcludedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelCompleter));
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

        public object GetState() => new PlayerSavableData(SelectedShip.AnchorName);

        public void SetState(object state)
        {
            if (state is null)
            {
                throw new EmptySavableStateEntryException(typeof(PlayerSavableData));
            }

            if (state is PlayerSavableData value)
            {
                if (_objectPoolEntryLookupTable.TryGetEntryByName(value.SelectedShipAnchorName, out var entry) == true)
                {
                    SelectedShip = entry;
                }
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(), typeof(PlayerSavableData), GetType());
            }
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

                if (value > 0f)
                {
                    _shipShootingController.SwitchToNextWeapons();
                }

                if (value < 0f)
                {
                    _shipShootingController.SwitchToPreviousWeapons();
                }
            }
        }

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            SelectedShip.EnsureObjectPoolExistence();
            _activeShip = s_multiobjectPool.Access.GetObject(SelectedShip.AnchorName);

            if (_activeShip.TryGetComponent(out IMovementController movementController) == true)
            {
                _shipMovementController = movementController;
            }
            else
            {
                throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IMovementController)}!");
            }

            if (_activeShip.TryGetComponent(out IShootingController shootingController) == true)
            {
                _shipShootingController = shootingController;
            }
            else
            {
                throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IShootingController)}!");
            }

            _gameControls.Gameplay.Enable();
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            if (_activeShip != null)
            {
                s_multiobjectPool.Access.ReleaseObject(SelectedShip.AnchorName, _activeShip);

                _activeShip = null;
                _shipMovementController = null;
            }
        }

        private void LevelConcludedEventHandler(object sender, EventArgs e)
        {
            _gameControls.Gameplay.Disable();
            _shipShootingController.StopShooting();
        }

        #endregion

        public void SetSelectedPlayerShip(ObjectPoolEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry), "Attempted to pass an empty selected ship!");
            }

            SelectedShip = entry;
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}