using SpaceAce.Architecture;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class MainMenuDisplay : UIDisplay, IRunnable
    {
        public override string DisplayHolderName => "Main menu display";

        public MainMenuDisplay(UIAssets assets) : base(assets.MainMenu, assets.Settings, assets.UIAudio) { }

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

            DisplayedDocument.visualTreeAsset = Display;

            DisplayedDocument.rootVisualElement.Q<Button>("Play-button").clicked += PlayButtonClickedEventHandler;
            DisplayedDocument.rootVisualElement.Q<Button>("Inventory-button").clicked += InventoryButtonClickedEventHandler;
        }

        protected override void Disable()
        {
            base.Disable();

            DisplayedDocument.rootVisualElement.Q<Button>("Play-button").clicked -= PlayButtonClickedEventHandler;
            DisplayedDocument.rootVisualElement.Q<Button>("Inventory-button").clicked -= InventoryButtonClickedEventHandler;

            DisplayedDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void PlayButtonClickedEventHandler()
        {
            Disable();
            UIAudio.ForwardButtonClick.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<LevelSelectionDisplay>(out var display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(LevelSelectionDisplay));
        }

        private void InventoryButtonClickedEventHandler()
        {
            Disable();
            UIAudio.ForwardButtonClick.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<InventoryDisplay>(out var display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(InventoryDisplay));
        }

        #endregion
    }
}