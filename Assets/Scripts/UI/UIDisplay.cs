using SpaceAce.Architecture;
using SpaceAce.Main.Audio;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public abstract class UIDisplay : IGameService
    {
        public event EventHandler Enabled, Disabled;

        public abstract string DisplayHolderName { get; }
        public bool Active { get; private set; } = false;

        protected VisualTreeAsset Display { get; private set; }
        protected UIDocument DisplayDocument { get; private set; }
        protected PanelSettings Settings { get; private set; }
        protected AudioCollection ButtonClickAudio { get; private set; }

        public UIDisplay(VisualTreeAsset display, PanelSettings settings, AudioCollection buttonClickAudio)
        {
            if (display == null) throw new ArgumentNullException(nameof(display), "Attempted to pass an empty UI document!");
            Display = display;

            if (settings == null) throw new ArgumentNullException(nameof(settings), "Attempted to pass an empty display settings!");
            Settings = settings;

            if (buttonClickAudio == null) throw new ArgumentNullException(nameof(buttonClickAudio), "Attempted to pass an empty button click audio collection!");
            ButtonClickAudio = buttonClickAudio;

            GameObject uiDisplay = new(DisplayHolderName);

            DisplayDocument = uiDisplay.AddComponent<UIDocument>();
            DisplayDocument.panelSettings = settings;
        }

        public abstract void OnInitialize();

        public abstract void OnSubscribe();

        public abstract void OnUnsubscribe();

        public abstract void OnClear();

        public virtual void Enable()
        {
            if (Active) return;

            Enabled?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        protected virtual void Disable()
        {
            if (Active == false) return;

            Disabled?.Invoke(this, EventArgs.Empty);
            Active = false;
        }
    }
}