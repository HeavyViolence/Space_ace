using System.Collections.Generic;

namespace SpaceAce.Gameplay.Inventory
{
    public static class SpecialEffectsMediator
    {
        private static readonly HashSet<object> s_receivers = new();

        public static bool Register(object receiver) => s_receivers.Add(receiver);
        public static bool Deregister(object receiver) => s_receivers.Remove(receiver);

        public static bool TryGetFirstEffectReceiver<T>(out T receiver)
        {
            foreach (var candidate in s_receivers)
            {
                if (candidate is T value)
                {
                    receiver = value;
                    return true;
                }
            }

            receiver = default;
            return false;
        }

        public static bool TryGetEffectReceivers<T>(out IEnumerable<T> receivers)
        {
            List<T> requestedReceivers = new(s_receivers.Count);

            foreach (var candidate in s_receivers)
            {
                if (candidate is T receiver) requestedReceivers.Add(receiver);
            }

            if (requestedReceivers.Count > 0)
            {
                receivers = requestedReceivers;
                return true;
            }
            else
            {
                receivers = null;
                return false;
            }
        }
    }
}