using SpaceAce.Architecture;
using SpaceAce.Main;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class PauseDisplay : UIDisplay
    {
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        private readonly GameControls _gameControls;

        public override string DisplayHolderName => "Pause display";

        public PauseDisplay(UIAssets assets) : base(assets.PauseMenu, assets.Settings, assets.ButtonClickAudio)
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

            s_gamePauser.Access.Pause();

            DisplayDocument.visualTreeAsset = Display;

            _gameControls.Menu.Back.Enable();
            _gameControls.Menu.Back.performed += BackButtonClickedEventHandler;

            DisplayDocument.rootVisualElement.Q<Button>("Resume-button").clicked += () => BackButtonClickedEventHandler(new());
            DisplayDocument.rootVisualElement.Q<Button>("Settings-button").clicked += SettingsButtonClickedEventHandler;
            DisplayDocument.rootVisualElement.Q<Button>("Quit-level-button").clicked += QuitLevelButtonClickedEventHandler;
        }

        protected override void Disable()
        {
            base.Disable();

            s_gamePauser.Access.Resume();

            _gameControls.Menu.Back.Disable();
            _gameControls.Menu.Back.performed -= BackButtonClickedEventHandler;

            DisplayDocument.rootVisualElement.Q<Button>("Resume-button").clicked -= () => BackButtonClickedEventHandler(new());
            DisplayDocument.rootVisualElement.Q<Button>("Settings-button").clicked -= SettingsButtonClickedEventHandler;
            DisplayDocument.rootVisualElement.Q<Button>("Quit-level-button").clicked -= QuitLevelButtonClickedEventHandler;

            DisplayDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void BackButtonClickedEventHandler(InputAction.CallbackContext ctx)
        {
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);
            Disable();

            if (GameServices.TryGetService(out HUDDisplay display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
        }

        private void SettingsButtonClickedEventHandler()
        {

        }

        private void QuitLevelButtonClickedEventHandler()
        {
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);
            Disable();

            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LoadMainMenu();
                CoroutineRunner.RunRoutine(MainMenuLoadingAwaiter(loader));
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
        }

        private static IEnumerator MainMenuLoadingAwaiter(GameModeLoader loader)
        {
            while (loader.GameState != GameState.MainMenu) yield return null;

            if (GameServices.TryGetService(out MainMenuDisplay display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(MainMenuDisplay));

            if (GameServices.TryGetService(out GamePauser pauser) == true) pauser.Resume();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(GamePauser));
        }

        #endregion
    }
}