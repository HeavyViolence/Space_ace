using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Levelry;
using SpaceAce.Main;
using SpaceAce.Main.ObjectPooling;
using SpaceAce.Main.Saving;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    public sealed class Player : IInitializable, ISavable, IFixedUpdatable
    {
        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();

        public event EventHandler SavingRequested;

        private ObjectPoolEntryLookupTable _objectPoolEntryLookupTable;
        private GameControls _gameControls;
        private IPlayerShipMovementController _playerShipMovementController;
        private GameObject _activePlayerShip;

        public string ID { get; }
        public string SaveName => "Player";
        public ObjectPoolEntry SelectedPlayerShip { get; private set; }

        public Player(string id,
                      ObjectPoolEntry defaultPlayerShip,
                      ObjectPoolEntryLookupTable table)
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

            SelectedPlayerShip = defaultPlayerShip;

            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), $"Attempted to pass an empty {nameof(ObjectPoolEntryLookupTable)}!");
            }

            _objectPoolEntryLookupTable = table;

            _gameControls = new();
            _gameControls.Menus.Enable();
            _gameControls.Gameplay.Disable();
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
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

        public void OnFixedUpdate()
        {
            if (_gameControls.Gameplay.enabled)
            {
                var movementDirection = _gameControls.Gameplay.Movement.ReadValue<Vector2>();
                _playerShipMovementController.Move(movementDirection);
            }
        }

        public object GetState() => new PlayerSavableData(SelectedPlayerShip.AnchorName);

        public void SetState(object state)
        {
            if (state is null)
            {
                throw new EmptySavableStateEntryException(typeof(PlayerSavableData));
            }

            if (state is PlayerSavableData value)
            {
                if (_objectPoolEntryLookupTable.TryGetEntryByName(value.SelectedPlayerShipAnchorName, out var entry) == true)
                {
                    SelectedPlayerShip = entry;
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

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            SelectedPlayerShip.EnsureObjectPoolExistence();
            _activePlayerShip = s_multiobjectPool.Access.GetObject(SelectedPlayerShip.AnchorName);

            if (_activePlayerShip.TryGetComponent(out IPlayerShipMovementController controller) == true)
            {
                _playerShipMovementController = controller;
            }
            else
            {
                throw new MissingComponentException($"Player ship is missing a mandatory component of type {typeof(IPlayerShipMovementController)}!");
            }

            _gameControls.Gameplay.Enable();
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            if (_activePlayerShip != null)
            {
                s_multiobjectPool.Access.ReleaseObject(SelectedPlayerShip.AnchorName, _activePlayerShip);
                _activePlayerShip = null;
            }
        }

        private void LevelConcludedEventHandler(object sender, EventArgs e)
        {
            _gameControls.Gameplay.Disable();
        }

        #endregion

        public void SetSelectedPlayerShip(ObjectPoolEntry entry)
        {
            SelectedPlayerShip = entry;
            SavingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}