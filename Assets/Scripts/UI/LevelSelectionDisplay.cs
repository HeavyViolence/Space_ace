using SpaceAce.Architecture;
using SpaceAce.Levels;
using SpaceAce.Main;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class LevelSelectionDisplay : UIDisplay
    {
        private static readonly GameServiceFastAccess<LevelUnlocker> s_levelUnlocker = new();
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();
        private static readonly GameServiceFastAccess<BestLevelsRunsStatisticsCollector> s_bestLevelsRunsStatisticsCollector = new();

        private int _selectedLevelIndex;
        private readonly GameControls _gameControls;

        public override string DisplayHolderName => "Level selection display";

        public LevelSelectionDisplay(UIAssets assets) : base(assets.LevelSelectionMenu, assets.Settings, assets.ButtonClickAudio)
        {
            _gameControls = new();
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

        public override void Enable()
        {
            base.Enable();

            DisplayDocument.visualTreeAsset = Display;

            _gameControls.Menu.Back.Enable();
            _gameControls.Menu.Back.performed += (c) => BackButtonClickedEventHandler();

            UpdateBestLevelRunDisplay(BestLevelRunStatistics.Default);

            var playButton = DisplayDocument.rootVisualElement.Q<Button>("play-selected-level-button");
            playButton.SetEnabled(false);
            playButton.clicked += PlayButtonClickedEventHandler;

            var backButton = DisplayDocument.rootVisualElement.Q<Button>("back-button");
            backButton.clicked += BackButtonClickedEventHandler;

            for (int i = 1; i <= LevelConfig.MaxLevelIndex; i++)
            {
                var levelButton = DisplayDocument.rootVisualElement.Q<Button>($"level-{i}-button");

                levelButton.SetEnabled(s_levelUnlocker.Access.IsLevelUnlocked(i));
                levelButton.clicked += () => LevelButtonClickedEventHandler(levelButton, playButton);
            }
        }

        protected override void Disable()
        {
            base.Disable();

            _gameControls.Menu.Back.Disable();
            _gameControls.Menu.Back.performed -= (c) => BackButtonClickedEventHandler();

            var playButton = DisplayDocument.rootVisualElement.Q<Button>("play-selected-level-button");
            playButton.clicked -= PlayButtonClickedEventHandler;

            var backButton = DisplayDocument.rootVisualElement.Q<Button>("back-button");
            backButton.clicked -= BackButtonClickedEventHandler;

            for (int i = 1; i <= LevelConfig.MaxLevelIndex; i++)
            {
                var levelButton = DisplayDocument.rootVisualElement.Q<Button>($"level-{i}-button");

                levelButton.clicked -= () => LevelButtonClickedEventHandler(levelButton, playButton);
            }

            DisplayDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void PlayButtonClickedEventHandler()
        {
            s_gameModeLoader.Access.LoadLevel(_selectedLevelIndex);
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);
            Disable();

            if (GameServices.TryGetService<HUDDisplay>(out var display) == true)
            {
                CoroutineRunner.RunRoutine(() => display.Enable(), () => s_gameModeLoader.Access.GameState == GameState.Level);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
            }
        }

        private void LevelButtonClickedEventHandler(Button levelButton, Button playButton)
        {
            _selectedLevelIndex = Convert.ToInt32(levelButton.text);

            if (playButton.enabledInHierarchy == false) playButton.SetEnabled(true);

            var selectedLevelStatistics = s_bestLevelsRunsStatisticsCollector.Access.GetStatistics(_selectedLevelIndex);
            UpdateBestLevelRunDisplay(selectedLevelStatistics);

            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);
        }

        private void BackButtonClickedEventHandler()
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<MainMenuDisplay>(out var display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(MainMenuDisplay));
        }

        private void UpdateBestLevelRunDisplay(BestLevelRunStatistics statistics)
        {
            if (statistics is null) throw new ArgumentNullException(nameof(statistics), $"Attempted to pass an empty {nameof(BestLevelRunStatistics)}!");

            var timeSpentLabel = DisplayDocument.rootVisualElement.Q<Label>("time-spent-label");
            var crystalsEarnedLabel = DisplayDocument.rootVisualElement.Q<Label>("credits-earned-label");
            var experienceEarnedLabel = DisplayDocument.rootVisualElement.Q<Label>("experience-earned-label");
            var enemiesDefeatedLabel = DisplayDocument.rootVisualElement.Q<Label>("enemies-defeated-label");

            var shootingAccuracyIndicatorLabel = DisplayDocument.rootVisualElement.Q<Label>("shooting-accuracy-indicator-label");
            var shootingAccuracyIndicator = DisplayDocument.rootVisualElement.Q<VisualElement>("shooting-accuracy-indicator-foreground");

            var playerDamageIndicatorLabel = DisplayDocument.rootVisualElement.Q<Label>("player-damage-indicator-label");
            var playerDamageIndicator = DisplayDocument.rootVisualElement.Q<VisualElement>("player-damage-indicator-foreground");

            var meteorsCrushedIndicatorLabel = DisplayDocument.rootVisualElement.Q<Label>("meteors-crushed-indicator-label");
            var meteorsCrushedIndicator = DisplayDocument.rootVisualElement.Q<VisualElement>("meteors-crushed-indicator-foreground");

            var spaceDebrisCrushedIndicatorLabel = DisplayDocument.rootVisualElement.Q<Label>("space-debris-crushed-indicator-label");
            var spaceDebrisCrushedIndicator = DisplayDocument.rootVisualElement.Q<VisualElement>("space-debris-crushed-indicator-foreground");

            var levelMasteryIndicatorLabel = DisplayDocument.rootVisualElement.Q<Label>("level-mastery-indicator-label");
            var levelMasteryIndicator = DisplayDocument.rootVisualElement.Q<VisualElement>("level-mastery-indicator-foreground");

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
                crystalsEarnedLabel.text = statistics.CreditsEarned.ToString("###,###");
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