using SpaceAce.Architecture;
using System;
using System.Collections.Generic;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class SpecialEffectsMediator : IGameService
    {
        private readonly HashSet<object> s_receivers = new();

        public Action<object> RegisteredReceiverBehaviourUpdate;

        public SpecialEffectsMediator() { }

        public bool Register(object receiver)
        {
            if (s_receivers.Add(receiver) == true)
            {
                RegisteredReceiverBehaviourUpdate?.Invoke(receiver);
                return true;
            }

            return false;
        }

        public bool Deregister(object receiver) => s_receivers.Remove(receiver);

        public bool TryGetFirstEffectReceiver<T>(out T receiver)
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

        public bool TryGetEffectReceivers<T>(out IEnumerable<T> receivers)
        {
            List<T> requestedReceivers = new(s_receivers.Count);

            foreach (var candidate in s_receivers) if (candidate is T receiver) requestedReceivers.Add(receiver);

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

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {

        }

        public void OnUnsubscribe()
        {

        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }
    }
}