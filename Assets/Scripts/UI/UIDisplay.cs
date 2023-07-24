using SpaceAce.Architecture;
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
        protected UIDocument DisplayedDocument { get; private set; }
        protected PanelSettings Settings { get; private set; }
        protected UIAudio UIAudio { get; private set; }

        public UIDisplay(VisualTreeAsset display, PanelSettings settings, UIAudio audio)
        {
            if (display == null) throw new ArgumentNullException(nameof(display));
            Display = display;

            if (settings == null) throw new ArgumentNullException(nameof(settings));
            Settings = settings;

            if (audio == null) throw new ArgumentNullException(nameof(audio));
            UIAudio = audio;

            GameObject uiDisplay = new(DisplayHolderName);

            DisplayedDocument = uiDisplay.AddComponent<UIDocument>();
            DisplayedDocument.panelSettings = settings;
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