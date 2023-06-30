using SpaceAce.Auxiliary;
using System;

namespace SpaceAce.Gameplay.Players
{
    public sealed class Experience
    {
        public event EventHandler<FloatValueChangedEventArgs> ValueChanged;

        public float Value { get; private set; }

        public Experience(float value = 0f)
        {
            if (value < 0f) throw new ArgumentOutOfRangeException(nameof(value), "Attempted to pass a negative value to a new experience account!");
            if (value > 0f) Value = value;
        }

        public void Add(float amount)
        {
            if (amount <= 0f) throw new ArgumentOutOfRangeException(nameof(amount), "Attempted to add a non-positive experience amount!");

            float oldValue = Value;
            float newValue = Value + amount;

            Value += amount;
            ValueChanged?.Invoke(this, new(oldValue, newValue));
        }

        public bool TryWithdraw(float amount)
        {
            if (amount <= 0f) throw new ArgumentOutOfRangeException(nameof(amount), "Cannot withdraw negative experience!");

            if (Value >= amount)
            {
                float oldValue = Value;
                float newValue = Value - amount;

                Value -= amount;
                ValueChanged?.Invoke(this, new(oldValue, newValue));

                return true;
            }

            return false;
        }
    }
}