using System;

namespace SpaceAce
{
    namespace Auxiliary
    {
        public sealed class Timer
        {
            public const float MinDuration = 0.1f;

            public event Action Started, Paused, Unpaused, OnReset, Elapsed;

            public float Duration { get; }
            public float Value { get; private set; }
            public float Progress => Value / Duration;
            public bool TimeIsUp => Progress >= 1f;
            public bool IsRunning { get; private set; }
            public bool IsPaused { get; private set; }

            public Timer(float duration)
            {
                Duration = Math.Clamp(duration, MinDuration, float.MaxValue);
                Value = 0f;
                IsRunning = false;
                IsPaused = false;
            }

            public void Start()
            {
                if (TimeIsUp == false && IsRunning == false)
                {
                    IsRunning = true;
                    Started?.Invoke();
                }
            }

            public void Pause()
            {
                if (IsRunning && IsPaused == false)
                {
                    IsPaused = true;
                    Paused?.Invoke();
                }
            }

            public void Unpause()
            {
                if (IsRunning && IsPaused)
                {
                    IsPaused = false;
                    Unpaused?.Invoke();
                }
            }

            public void Reset()
            {
                IsRunning = false;
                IsPaused = false;
                Value = 0f;
                OnReset?.Invoke();
            }

            public void Tick(float tickDuration)
            {
                if (IsRunning && IsPaused == false)
                {
                    if (TimeIsUp == false)
                    {
                        Value += tickDuration;
                    }
                    else
                    {
                        IsRunning = false;
                        Elapsed?.Invoke();
                    }
                }
            }
        }
    }
}