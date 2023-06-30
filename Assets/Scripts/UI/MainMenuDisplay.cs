using SpaceAce.Architecture;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class MainMenuDisplay : UIDisplay, IRunnable
    {
        public override string DisplayHolderName => "Main menu display";

        public MainMenuDisplay(UIAssets assets) : base(assets.MainMenu, assets.Settings, assets.ButtonClickAudio) { }

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

        public void OnRun() => Enable();

        public override void OnClear()
        {
            GameServices.Deregister(this);
        }

        public override void Enable()
        {
            base.Enable();

            DisplayDocument.visualTreeAsset = Display;

            DisplayDocument.rootVisualElement.Q<Button>("play-button").clicked += PlayButtonClickedEventHandler;
            DisplayDocument.rootVisualElement.Q<Button>("inventory-button").clicked += InventoryButtonClickedEventHandler;
        }

        protected override void Disable()
        {
            base.Disable();

            DisplayDocument.rootVisualElement.Q<Button>("play-button").clicked -= PlayButtonClickedEventHandler;
            DisplayDocument.rootVisualElement.Q<Button>("inventory-button").clicked -= InventoryButtonClickedEventHandler;

            DisplayDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void PlayButtonClickedEventHandler()
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<LevelSelectionDisplay>(out var display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelSelectionDisplay));
        }

        private void InventoryButtonClickedEventHandler()
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<InventoryDisplay>(out var display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(InventoryDisplay));
        }

        #endregion
    }
}