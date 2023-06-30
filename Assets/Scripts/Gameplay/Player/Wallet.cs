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
            if (credits < 0) throw new ArgumentOutOfRangeException(nameof(credits), "Attempted to set a negative balance to a new wallet!");
            if (credits > 0) Credits = credits;
        }

        public void AddCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Attempted to add a non-positive amount of credits to the wallet!");

            int oldValue = Credits;
            int newValue = Credits + amount;

            Credits += amount;
            BalanceChanged?.Invoke(this, new(oldValue, newValue));
        }

        public bool TryBuy(int price)
        {
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative!");

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