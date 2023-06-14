using SpaceAce.Architecture;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class HUDDisplay : UIDisplay
    {
        private readonly GameControls _gameControls;

        private VisualElement _overdriveDisplay;
        private VisualElement _powerupDisplay;
        private VisualElement _damageableEntityDisplay;

        public override string DisplayHolderName => "HUD display";

        public HUDDisplay(UIAssets assets) : base(assets.HUDDisplay, assets.Settings, assets.ButtonClickAudio)
        {
            _gameControls = new GameControls();
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

            _overdriveDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Overdrive-display");
            _powerupDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Powerup-display");
            _damageableEntityDisplay = DisplayDocument.rootVisualElement.Q<VisualElement>("Damageable-entity-display");

            _overdriveDisplay.style.display = DisplayStyle.None;
            _powerupDisplay.style.display = DisplayStyle.None;
            _damageableEntityDisplay.style.display = DisplayStyle.None;

            _gameControls.Menu.Enable();
            _gameControls.Menu.Back.performed += BackButtonClickedEventHandler;
            _gameControls.Menu.Inventory.performed += InventoryButtonClickedEventHandler;
        }

        protected override void Disable()
        {
            base.Disable();

            _gameControls.Menu.Disable();
            _gameControls.Menu.Back.performed -= BackButtonClickedEventHandler;
            _gameControls.Menu.Inventory.performed -= InventoryButtonClickedEventHandler;

            DisplayDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void BackButtonClickedEventHandler(InputAction.CallbackContext context)
        {
            /*Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);*/

            //Implement pause display to proceed here
        }

        private void InventoryButtonClickedEventHandler(InputAction.CallbackContext context)
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (GameServices.TryGetService<InventoryDisplay>(out var display) == true)
            {
                display.Enable();
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(InventoryDisplay));
            }
        }

        #endregion
    }
}