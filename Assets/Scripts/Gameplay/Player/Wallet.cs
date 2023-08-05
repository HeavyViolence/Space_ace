using SpaceAce.Auxiliary;
using System;

namespace SpaceAce.Gameplay.Players
{
    public sealed class Wallet
    {
        public event EventHandler<IntValueChangedEventArgs> BalanceChanged;

        public int Credits { get; private set; } = 0;

        public Wallet(int credits = 0)
        {
            if (credits < 0) throw new ArgumentOutOfRangeException(nameof(credits));
            if (credits > 0) Credits = credits;
        }

        public void AddCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

            int oldValue = Credits;
            int newValue = Credits + amount;

            Credits += amount;
            BalanceChanged?.Invoke(this, new(oldValue, newValue));
        }

        public bool TryBuy(int price)
        {
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));

            if (Credits >= price)
            {
                int oldValue = Credits;
                int newValue = Credits - price;

                Credits -= price;
                BalanceChanged?.Invoke(this, new(oldValue, newValue));

                return true;
            }

            return false;
        }
    }
}