using SpaceAce.Architecture;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceAce.UI
{
    public sealed class PauseDisplay : UIDisplay
    {
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

            _gameControls.Menu.Back.Enable();
            _gameControls.Menu.Back.performed += BackButtonClickedEventHandler;
        }

        protected override void Disable()
        {
            base.Disable();

            _gameControls.Menu.Back.Disable();
            _gameControls.Menu.Back.performed -= BackButtonClickedEventHandler;
        }

        #region event handlers

        private void BackButtonClickedEventHandler(InputAction.CallbackContext obj)
        {
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);
            Disable();

            if (GameServices.TryGetService(out HUDDisplay display) == true) display.Enable();
            else throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
        }

        #endregion
    }
}