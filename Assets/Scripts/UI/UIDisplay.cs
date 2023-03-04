using SpaceAce.Architecture;
using SpaceAce.Levelry;
using SpaceAce.Main;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class UIDisplay : IInitializable
    {
        private UIAssets _uiAssets;
        private UIDocument _activeUI;

        private GameServiceFastAccess<LevelUnlocker> _levelUnlocker = new();
        private GameServiceFastAccess<GameModeLoader> _gameModeLoader = new();
        private GameServiceFastAccess<BestLevelsRunsStatisticsCollector> _bestLevelsRunsStatisticsCollector = new();

        private int _selectedLevelIndex;

        public UIDisplay(UIAssets assets)
        {
            if (assets == null)
            {
                throw new ArgumentNullException(nameof(assets), $"Attempted to pass an empty {nameof(UIAssets)}!");
            }

            _uiAssets = assets;

            GameObject uiDisplay = new("UI display");
            _activeUI = uiDisplay.AddComponent<UIDocument>();

            _activeUI.panelSettings = _uiAssets.Settings;

            EnableMainMenu();
        }

        #region menu schedule

        private void EnableMainMenu()
        {
            _activeUI.visualTreeAsset = _uiAssets.MainMenu;

            var playButton = _activeUI.rootVisualElement.Q<Button>("play-button");
            playButton.clicked += MainMenuPlayButtonClickedEventHandler;
        }

        private void DisableMainMenu()
        {
            var playButton = _activeUI.rootVisualElement.Q<Button>("play-button");
            playButton.clicked -= MainMenuPlayButtonClickedEventHandler; ;

            _activeUI.visualTreeAsset = null;
        }

        private void EnablePlayMenu()
        {
            _activeUI.visualTreeAsset = _uiAssets.PlayMenu;

            UpdateLevelStatisticsDisplay(BestLevelRunStatistics.Default);

            var playButton = _activeUI.rootVisualElement.Q<Button>("play-selected-level-button");
            playButton.SetEnabled(false);
            playButton.clicked += PlayMenuPlayButtonClickedEventHandler;

            var backButton = _activeUI.rootVisualElement.Q<Button>("back-button");
            backButton.clicked += PlayMenuBackButtonCkickedEventHandler;

            for (int i = 1; i <= LevelConfig.MaxLevelIndex; i++)
            {
                var levelButton = _activeUI.rootVisualElement.Q<Button>($"level-{i}-button");

                levelButton.SetEnabled(_levelUnlocker.Access.IsLevelUnlocked(i));
                levelButton.clicked += () => LevelButtonClickedEventHandler(levelButton, playButton);
            }
        }

        private void DisablePlayMenu()
        {
            var playButton = _activeUI.rootVisualElement.Q<Button>("play-selected-level-button");
            playButton.clicked -= PlayMenuPlayButtonClickedEventHandler;

            var backButton = _activeUI.rootVisualElement.Q<Button>("back-button");
            backButton.clicked -= PlayMenuBackButtonCkickedEventHandler;

            for (int i = 1; i <= LevelConfig.MaxLevelIndex; i++)
            {
                var levelButton = _activeUI.rootVisualElement.Q<Button>($"level-{i}-button");

                levelButton.clicked -= () => LevelButtonClickedEventHandler(levelButton, playButton);
            }

            _activeUI.visualTreeAsset = null;
        }

        private void EnableSettingsMenu()
        {

        }

        private void DisableSettingsMenu()
        {

        }

        private void EnableInventoryMenu()
        {

        }

        private void DisableInventoryMenu()
        {

        }

        private void EnableArmoryMenu()
        {

        }

        private void DisableArmoryMenu()
        {

        }

        private void EnableCreditsMenu()
        {

        }

        private void DisableCreditsMenu()
        {

        }

        #endregion

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {

        }

        public void OnUnsubscribe()
        {

        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        #endregion

        #region event handlers

        private void MainMenuPlayButtonClickedEventHandler()
        {
            DisableMainMenu();
            EnablePlayMenu();
        }

        private void PlayMenuPlayButtonClickedEventHandler()
        {
            DisablePlayMenu();
            _gameModeLoader.Access.LoadLevel(_selectedLevelIndex);
        }

        private void PlayMenuBackButtonCkickedEventHandler()
        {
            DisablePlayMenu();
            EnableMainMenu();
        }

        private void LevelButtonClickedEventHandler(Button levelButton, Button playButton)
        {
            _selectedLevelIndex = Convert.ToInt32(levelButton.text);

            if (playButton.enabledInHierarchy == false)
            {
                playButton.SetEnabled(true);
            }

            var selectedLevelStatistics = _bestLevelsRunsStatisticsCollector.Access.GetStatistics(_selectedLevelIndex);
            UpdateLevelStatisticsDisplay(selectedLevelStatistics);
        }

        private void UpdateLevelStatisticsDisplay(BestLevelRunStatistics statistics)
        {
            if (statistics is null)
            {
                throw new ArgumentNullException(nameof(statistics), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");
            }

            var timeSpentLabel = _activeUI.rootVisualElement.Q<Label>("time-spent-label");
            var crystalsEarnedLabel = _activeUI.rootVisualElement.Q<Label>("crystals-earned-label");
            var experienceEarnedLabel = _activeUI.rootVisualElement.Q<Label>("experience-earned-label");
            var enemiesDefeatedLabel = _activeUI.rootVisualElement.Q<Label>("enemies-defeated-label");

            var shootingAccuracyIndicatorLabel = _activeUI.rootVisualElement.Q<Label>("shooting-accuracy-indicator-label");
            var shootingAccuracyIndicator = _activeUI.rootVisualElement.Q<VisualElement>("shooting-accuracy-indicator-foreground");

            var playerDamageIndicatorLabel = _activeUI.rootVisualElement.Q<Label>("player-damage-indicator-label");
            var playerDamageIndicator = _activeUI.rootVisualElement.Q<VisualElement>("player-damage-indicator-foreground");

            var meteorsCrushedIndicatorLabel = _activeUI.rootVisualElement.Q<Label>("meteors-crushed-indicator-label");
            var meteorsCrushedIndicator = _activeUI.rootVisualElement.Q<VisualElement>("meteors-crushed-indicator-foreground");

            var spaceDebrisCrushedIndicatorLabel = _activeUI.rootVisualElement.Q<Label>("space-debris-crushed-indicator-label");
            var spaceDebrisCrushedIndicator = _activeUI.rootVisualElement.Q<VisualElement>("space-debris-crushed-indicator-foreground");

            var levelMasteryIndicatorLabel = _activeUI.rootVisualElement.Q<Label>("level-mastery-indicator-label");
            var levelMasteryIndicator = _activeUI.rootVisualElement.Q<VisualElement>("level-mastery-indicator-foreground");

            if (statistics == BestLevelRunStatistics.Default)
            {
                timeSpentLabel.text = "00:00";
                crystalsEarnedLabel.text = "0";
                experienceEarnedLabel.text = "0";
                enemiesDefeatedLabel.text = "0";

                shootingAccuracyIndicatorLabel.text = "Shooting accuracy (0%)";
                shootingAccuracyIndicator.style.width = new(Length.Percent(0f));

                playerDamageIndicatorLabel.text = "Player damage (0%)";
                playerDamageIndicator.style.width = new(Length.Percent(0f));

                meteorsCrushedIndicatorLabel.text = "Meteors crushed (0%)";
                meteorsCrushedIndicator.style.width = new(Length.Percent(0f));

                spaceDebrisCrushedIndicatorLabel.text = "Space debris crushed (0%)";
                spaceDebrisCrushedIndicator.style.width = new(Length.Percent(0f));

                levelMasteryIndicatorLabel.text = "Level mastery (0%)";
                levelMasteryIndicator.style.width = new(Length.Percent(0f));
            }
            else
            {
                timeSpentLabel.text = statistics.TimeSpent.minutes.ToString("##") + ":" +
                                      statistics.TimeSpent.seconds.ToString("##");
                crystalsEarnedLabel.text = statistics.CrystalsEarned.ToString("###,###");
                experienceEarnedLabel.text = statistics.ExperienceEarned.ToString("###,###,###");
                enemiesDefeatedLabel.text = statistics.EnemiesDefeated.ToString();

                shootingAccuracyIndicatorLabel.text = $"Shooting accuracy ({statistics.ShootingAccuracy * 100f:n2}%)";
                shootingAccuracyIndicator.style.width = new(Length.Percent(statistics.ShootingAccuracy * 100f));

                playerDamageIndicatorLabel.text = $"Player damage ({statistics.PlayerDamagePercentage * 100f:n2}%)";
                playerDamageIndicator.style.width = new(Length.Percent(statistics.PlayerDamagePercentage * 100f));

                meteorsCrushedIndicatorLabel.text = $"Meteors crushed ({statistics.MeteorsCrushedPercentage * 100f:n2}%)";
                meteorsCrushedIndicator.style.width = new(Length.Percent(statistics.MeteorsCrushedPercentage * 100f));

                spaceDebrisCrushedIndicatorLabel.text = $"Space debris crushed ({statistics.SpaceDebrisCrushedPercentage * 100f:n2}%)";
                spaceDebrisCrushedIndicator.style.width = new(Length.Percent(statistics.SpaceDebrisCrushedPercentage * 100f));

                levelMasteryIndicatorLabel.text = $"Level mastery ({statistics.LevelMastery * 100f:n2}%)";
                levelMasteryIndicator.style.width = new(Length.Percent(statistics.LevelMastery * 100f));
            }
        }

        #endregion
    }
}