using SpaceAce.Architecture;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceAce.Main
{
    public sealed class ScreenFader : IGameService
    {
        public const float MinFadeDuration = 0.5f;
        public const float MaxFadeDuration = 5f;
        public const float DefaultFadeDuration = 2f;

        public event EventHandler FadingStarted, FadedOut, FadingCompleted;

        private readonly Image _faderImage;

        private readonly Color32 _inactiveFadeColor;
        private readonly Color32 _activeFadeColor;

        private readonly AnimationCurve _fadingCurve;

        public bool Active { get; private set; } = false;

        public ScreenFader(AnimationCurve fadingCurve, Color32 fadingColor)
        {
            if (fadingCurve == null)
            {
                throw new ArgumentNullException(nameof(fadingCurve), "Attempted to pass an empty fading curve!");
            }

            _fadingCurve = fadingCurve;
            _inactiveFadeColor = new(fadingColor.r, fadingColor.g, fadingColor.b, 0);
            _activeFadeColor = fadingColor;

            GameObject canvasAnchor = new("Screen fader");

            Canvas faderCanvas = canvasAnchor.AddComponent<Canvas>();
            canvasAnchor.AddComponent<CanvasScaler>();
            canvasAnchor.AddComponent<GraphicRaycaster>();

            faderCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            faderCanvas.sortingOrder = short.MaxValue;

            GameObject imageAnchor = new("Image");
            imageAnchor.transform.parent = canvasAnchor.transform;

            _faderImage = imageAnchor.AddComponent<Image>();
            _faderImage.color = _inactiveFadeColor;

            RectTransform imageTransform = _faderImage.GetComponent<RectTransform>();
            imageTransform.localPosition = new Vector3(0f, 0f, 0f);
            imageTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }

        public void PerformScreenFading(float duration = DefaultFadeDuration)
        {
            if (Active == false)
            {
                float clampedDuration = Mathf.Clamp(duration, MinFadeDuration, MaxFadeDuration);
                CoroutineRunner.RunRoutine(Fader(clampedDuration));

                IEnumerator Fader(float duration)
                {
                    Active = true;
                    FadingStarted?.Invoke(this, EventArgs.Empty);

                    float timer = 0f;
                    float lerpFactor;
                    float halvedDuration = duration / 2f;
                    bool fadedOutEventCalled = false;

                    while (timer < duration)
                    {
                        timer += Time.deltaTime;
                        lerpFactor = _fadingCurve.Evaluate(timer / duration);

                        _faderImage.color = Color32.Lerp(_inactiveFadeColor, _activeFadeColor, lerpFactor);

                        if (timer > halvedDuration && fadedOutEventCalled == false)
                        {
                            FadedOut?.Invoke(this, EventArgs.Empty);
                            fadedOutEventCalled = true;
                        }

                        yield return null;
                    }

                    FadingCompleted?.Invoke(this, EventArgs.Empty);
                    Active = false;
                }
            }
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.MainMenuLoadingStarted += (s, e) => PerformScreenFading(e.Delay * 2f);
                loader.LevelLoadingStarted += (s, e) => PerformScreenFading(e.Delay * 2f);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.MainMenuLoadingStarted -= (s, e) => PerformScreenFading(e.Delay * 2f);
                loader.LevelLoadingStarted -= (s, e) => PerformScreenFading(e.Delay * 2f);
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
        }

        public void OnClear()
        {
            GameServices.Deregister(this);

            FadingStarted = null;
            FadedOut = null;
            FadingCompleted = null;
        }

        #endregion
    }
}