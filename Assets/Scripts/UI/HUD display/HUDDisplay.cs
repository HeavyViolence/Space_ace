using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Spawning;
using SpaceAce.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class HUDDisplay : UIDisplay, IUpdatable
    {
        private const float EasingDuration = 2f;

        private static readonly GameServiceFastAccess<EnemySpawner> s_enemySpawner = new();
        private static readonly GameServiceFastAccess<MeteorSpawner> s_meteorSpawner = new();
        private static readonly GameServiceFastAccess<SpaceDebrisSpawner> s_spaceDebrisSpawner = new();
        private static readonly GameServiceFastAccess<LevelTimer> s_levelTimer = new();
        private static readonly GameServiceFastAccess<LevelRewardCollector> s_levelRewardCollector = new();

        private readonly GameControls _gameControls;

        private readonly HashSet<EntityView> _entityViews;
        private EntityView _activeEntityView;
        private Coroutine _activeEntityViewRoutine;
        private EntityView _playerShipView;

        private VisualElement _overdriveDisplay;
        private VisualElement _powerupDisplay;

        private VisualElement _playerShipDisplay;
        private VisualElement _playerShipIcon;
        private VisualElement _playerShipHealthbar;
        private Label _playerShipMaxHealthLabel;
        private Label _playerShipArmorLabel;
        private Label _playerShipWeaponsLabel;

        private VisualElement _entityDisplay;
        private VisualElement _entityIcon;
        private VisualElement _entityHealthbar;
        private Label _entityMaxHealthLabel;
        private Label _entityArmorLabel;
        private VisualElement _entityWeaponsDisplay;
        private Label _entityWeaponsLabel;

        private Label _enemiesKilledLabel;
        private Label _meteorsKilledLabel;
        private Label _spaceDebrisKilledLabel;
        private Label _levelCreditRewardLabel;
        private Label _levelExperienceRewardLabel;
        private Label _levelTimeLabel;

        public override string DisplayHolderName => "HUD display";

        public HUDDisplay(UIAssets assets) : base(assets.HUDDisplay, assets.Settings, assets.ButtonClickAudio)
        {
            _gameControls = new();
            _entityViews = new();
            _activeEntityView = null;
            _activeEntityViewRoutine = null;
        }

        public bool RegisterEntityView(EntityView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view), $"Attempted to register an empty {typeof(EntityView)}!");

            if (_entityViews.Add(view) == true)
            {
                view.DisplayRequested += EntityViewDisplayRequestedEventHandler;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeregisterEntityView(EntityView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view), $"Attempted to deregister an empty {typeof(EntityView)}!");

            if (_entityViews.Remove(view) == true)
            {
                view.DisplayRequested -= EntityViewDisplayRequestedEventHandler;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RegisterPlayerView(EntityView playerView)
        {
            if (playerView == null) throw new ArgumentNullException(nameof(playerView), $"Attempted to register an empty {typeof(EntityView)}!");

            if (_playerShipView == null)
            {
                _playerShipView = playerView;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeregisterPlayerView()
        {
            if (_playerShipView != null)
            {
                _playerShipView = null;
                return true;
            }

            return false;
        }

        public override void OnInitialize()
        {
            GameServices.Register(this);
        }

        public override void OnSubscribe()
        {
            
        }

        public override void OnUnsubscribe()
        {
            
        }

        public override void OnClear()
        {
            GameServices.Deregister(this);
        }

        public void OnUpdate()
        {
            if (Active)
            {
                _enemiesKilledLabel.text = $"{s_enemySpawner.Access.DestroyedCount}/{s_enemySpawner.Access.ToSpawnCount}";
                _meteorsKilledLabel.text = $"{s_meteorSpawner.Access.DestroyedCount}/{s_meteorSpawner.Access.SpawnedCount}";
                _spaceDebrisKilledLabel.text = $"{s_spaceDebrisSpawner.Access.DestroyedCount}/{s_spaceDebrisSpawner.Access.SpawnedCount}";
                _levelTimeLabel.text = $"{s_levelTimer.Access.Minutes:##0}:{s_levelTimer.Access.Seconds:00}";

                UpdatePlayerShipViewDisplay();
            }
        }

        public override void Enable()
        {
            base.Enable();

            DisplayDocument.visualTreeAsset = Display;

            _overdriveDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Overdrive-display");
            _powerupDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Powerup-display");

            _enemiesKilledLabel = DisplayDocument.rootVisualElement.Q<Label>("Enemies-to-kill-label");
            _meteorsKilledLabel = DisplayDocument.rootVisualElement.Q<Label>("Meteors-killed-label");
            _spaceDebrisKilledLabel = DisplayDocument.rootVisualElement.Q<Label>("Space-debris-killed-label");
            _levelCreditRewardLabel = DisplayDocument.rootVisualElement.Q<Label>("Level-credit-reward-label");
            _levelExperienceRewardLabel = DisplayDocument.rootVisualElement.Q<Label>("Level-experience-reward-label");
            _levelTimeLabel = DisplayDocument.rootVisualElement.Q<Label>("Level-time-label");

            _playerShipDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Player-ship-display");
            _playerShipIcon = DisplayDocument.rootVisualElement.Q<VisualElement>("Player-ship-icon");
            _playerShipHealthbar = DisplayDocument.rootVisualElement.Q<VisualElement>("Player-ship-healthbar-foreground");
            _playerShipMaxHealthLabel = DisplayDocument.rootVisualElement.Q<Label>("Player-ship-max-health-label");
            _playerShipArmorLabel = DisplayDocument.rootVisualElement.Q<Label>("Player-ship-armor-label");
            _playerShipWeaponsLabel = DisplayDocument.rootVisualElement.Q<Label>("Player-ship-weapons-label");

            _entityDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Entity-display");
            _entityIcon = DisplayDocument.rootVisualElement.Q<VisualElement>("Entity-icon");
            _entityHealthbar = DisplayDocument.rootVisualElement.Q<VisualElement>("Entity-healthbar-foreground");
            _entityMaxHealthLabel = DisplayDocument.rootVisualElement.Q<Label>("Entity-max-health-label");
            _entityArmorLabel = DisplayDocument.rootVisualElement.Q<Label>("Entity-armor-label");
            _entityWeaponsDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Entity-weapons-display");
            _entityWeaponsLabel = DisplayDocument.rootVisualElement.Q<Label>("Entity-weapons-label");

            _overdriveDisplay.style.display = DisplayStyle.None;
            _powerupDisplay.style.display = DisplayStyle.None;
            _entityDisplay.style.display = DisplayStyle.None;
            _playerShipDisplay.style.display = DisplayStyle.None;

            _gameControls.Menu.Enable();
            _gameControls.Menu.Back.performed += BackButtonClickedEventHandler;
            _gameControls.Menu.Inventory.performed += InventoryButtonClickedEventHandler;

            s_levelRewardCollector.Access.ExperienceRewardChanged += (s, e) => CoroutineRunner.RunRoutine(UpdateExperienceRewardDisplay(e.OldValue, e.NewValue));
            s_levelRewardCollector.Access.CreditRewardChanged += (s, e) => CoroutineRunner.RunRoutine(UpdateCreditRewardDisplay(e.OldValue, e.NewValue));

            _levelCreditRewardLabel.text = $"{s_levelRewardCollector.Access.CreditReward:n0}";
            _levelExperienceRewardLabel.text = $"{s_levelRewardCollector.Access.ExperienceReward:n0}";

            _playerShipDisplay.style.display = DisplayStyle.None;
            _entityDisplay.style.display = DisplayStyle.None;
        }

        protected override void Disable()
        {
            base.Disable();

            _gameControls.Menu.Disable();
            _gameControls.Menu.Back.performed -= BackButtonClickedEventHandler;
            _gameControls.Menu.Inventory.performed -= InventoryButtonClickedEventHandler;

            s_levelRewardCollector.Access.ExperienceRewardChanged -= (s, e) => CoroutineRunner.RunRoutine(UpdateExperienceRewardDisplay(e.OldValue, e.NewValue));
            s_levelRewardCollector.Access.CreditRewardChanged -= (s, e) => CoroutineRunner.RunRoutine(UpdateCreditRewardDisplay(e.OldValue, e.NewValue));

            DisplayDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void BackButtonClickedEventHandler(InputAction.CallbackContext context)
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService(out PauseDisplay display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(PauseDisplay));
        }

        private void InventoryButtonClickedEventHandler(InputAction.CallbackContext context)
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<InventoryDisplay>(out var display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(InventoryDisplay));
        }

        private void EntityViewDisplayRequestedEventHandler(object sender, EventArgs e)
        {
            if (sender is EntityView view)
            {
                if (view == _activeEntityView) return;

                if (_activeEntityView != null)
                {
                    CoroutineRunner.StopRoutine(_activeEntityViewRoutine);
                    _activeEntityViewRoutine = null;
                }

                _activeEntityViewRoutine = CoroutineRunner.RunRoutine(UpdateEntityViewDisplay(view));
            }
            else
            {
                throw new ArgumentException($"{nameof(sender)} is being expected to be of a {typeof(EntityView)} type!", nameof(sender));
            }
        }

        #endregion

        private IEnumerator UpdateCreditRewardDisplay(float oldValue, float newValue)
        {
            float timer = 0f;
            float interpolationFactor;
            float valueToDisplay;

            while (timer < EasingDuration)
            {
                timer += Time.deltaTime;
                interpolationFactor = AuxMath.EasingCurveIn.Evaluate(timer / EasingDuration);
                valueToDisplay = Mathf.Lerp(oldValue, newValue, interpolationFactor);

                _levelCreditRewardLabel.text = $"{valueToDisplay:n0}";

                yield return null;
            }

            _levelCreditRewardLabel.text = $"{s_levelRewardCollector.Access.CreditReward:n0}";
        }

        private IEnumerator UpdateExperienceRewardDisplay(float oldValue, float newValue)
        {
            float timer = 0f;
            float interpolationFactor;
            float valueToDisplay;

            while (timer < EasingDuration)
            {
                timer += Time.deltaTime;
                interpolationFactor = AuxMath.EasingCurveIn.Evaluate(timer / EasingDuration);
                valueToDisplay = Mathf.Lerp(oldValue, newValue, interpolationFactor);

                _levelExperienceRewardLabel.text = $"{valueToDisplay:n0}";

                yield return null;
            }

            _levelExperienceRewardLabel.text = $"{s_levelRewardCollector.Access.ExperienceReward:n0}";
        }

        private IEnumerator UpdateEntityViewDisplay(EntityView view)
        {
            _activeEntityView = view;
            _entityDisplay.style.display = DisplayStyle.Flex;
            _entityIcon.style.backgroundImage = view.Icon.texture;
            _entityMaxHealthLabel.text = view.Health.RegenPerSecond == 0f ? $"{view.Health.MaxValue:n0}"
                                                                          : $"{view.Health.MaxValue:n0} (+{view.Health.RegenPerSecond:n0})";
            _entityArmorLabel.text = $"{view.Armor.Value:n0}";

            if (view.Weapons is null &&
                _entityWeaponsDisplay.style.display != DisplayStyle.None) _entityWeaponsDisplay.style.display = DisplayStyle.None;
            else _entityWeaponsDisplay.style.display = DisplayStyle.Flex;

            float timer = 0f;

            while (view.Active)
            {
                timer += Time.deltaTime;

                _entityHealthbar.style.width = new(Length.Percent(view.Health.ValuePercentage));

                if (view.Weapons is not null)
                    _entityWeaponsLabel.text = $"{view.Weapons.MaxDamagePerSecond:n0} ({view.Weapons.ActiveWeaponGroupIndex + 1}/{view.Weapons.WeaponGroupsAmount})";

                yield return null;
            }

            _entityDisplay.style.display = DisplayStyle.None;
            _activeEntityView = null;
        }

        private void UpdatePlayerShipViewDisplay()
        {
            if (_playerShipView == null &&
                _playerShipDisplay.style.display == DisplayStyle.Flex) _playerShipDisplay.style.display = DisplayStyle.None;

            if (_playerShipView != null &&
                _playerShipDisplay.style.display == DisplayStyle.None) _playerShipDisplay.style.display = DisplayStyle.Flex;

            _playerShipIcon.style.backgroundImage = _playerShipView.Icon.texture;

            _playerShipMaxHealthLabel.text = _playerShipView.Health.RegenPerSecond == 0f ? $"{_playerShipView.Health.MaxValue:n0}"
                                                                                         : $"{_playerShipView.Health.MaxValue:n0}" +
                                                                                           $" (+{_playerShipView.Health.RegenPerSecond:n0})";
            _playerShipArmorLabel.text = $"{_playerShipView.Armor.Value:n0}";

            _playerShipWeaponsLabel.text = $"{_playerShipView.Weapons.MaxDamagePerSecond:n0} " +
                $"({_playerShipView.Weapons.ActiveWeaponGroupIndex + 1}/{_playerShipView.Weapons.WeaponGroupsAmount})";

            _playerShipHealthbar.style.width = new(Length.Percent(_playerShipView.Health.ValuePercentage));
        }
    }
}