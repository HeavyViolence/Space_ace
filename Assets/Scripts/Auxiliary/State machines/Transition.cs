using System;

namespace SpaceAce.Auxiliary.StateMachines
{
    public sealed class Transition : IEquatable<Transition>
    {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public Transition(IState to, Func<bool> condition)
        {
            if (to is null)
            {
                throw new ArgumentNullException(nameof(to), "Attempted to pass an empty state to transition to!");
            }

            To = to;

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition), "Attempted to pass an empty state transition condition!");
            }

            Condition = condition;
        }

        public override bool Equals(object obj) => Equals(obj as Transition);

        public bool Equals(Transition other) => other is not null &&
                                                To.GetType().Equals(other.GetType()) &&
                                                Condition.Equals(other.Condition);

        public static bool operator ==(Transition x, Transition y)
        {
            if (x is null)
            {
                if (y is null)
                {
                    return true;
                }

                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(Transition x, Transition y) => !(x == y);

        public override int GetHashCode() => To.GetType().GetHashCode() ^ Condition.GetHashCode();
    }
}