using System;
using System.Collections.Generic;

namespace SpaceAce.Auxiliary
{
    public sealed class RangedInt : IEquatable<RangedInt>, IComparable<RangedInt>, IComparer<RangedInt>
    {
        public static RangedInt Zero => new(0, 0, 0, 0, 0);

        private readonly Random _random;

        public int Pivot { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public int RandomValue => _random.Next(MinValue, MaxValue + 1);
        public int RangeWidth => MaxValue - MinValue;
        public int Mean => MinValue + RangeWidth / 2;
        public bool IsZeroed => Equals(Zero);

        public RangedInt(int pivot, int range)
        {
            if (range < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = pivot - range;
            MaxValue = pivot + range;

            _random = new();
        }

        public RangedInt(int pivot, int range, int seed)
        {
            if (range < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = pivot - range;
            MaxValue = pivot + range;

            _random = new(seed);
        }

        public RangedInt(int pivot, int range, int min, int max)
        {
            if (range < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = Math.Clamp(pivot - range, min, int.MaxValue);
            MaxValue = Math.Clamp(pivot + range, int.MinValue, max);

            _random = new();
        }

        public RangedInt(int pivot, int range, int min, int max, int seed)
        {
            if (range < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = Math.Clamp(pivot - range, min, int.MaxValue);
            MaxValue = Math.Clamp(pivot + range, int.MinValue, max);

            _random = new(seed);
        }

        public override bool Equals(object obj) => Equals(obj as RangedInt);

        public bool Equals(RangedInt other) => other is not null && Mean.Equals(other.Mean);

        public override int GetHashCode() => Mean.GetHashCode();

        public int CompareTo(RangedInt other)
        {
            if (other is null)
            {
                return 1;
            }

            if (Mean < other.Mean)
            {
                return -1;
            }

            if (Mean > other.Mean)
            {
                return 1;
            }

            return 0;
        }

        public int Compare(RangedInt x, RangedInt y)
        {
            if (x.Mean < y.Mean)
            {
                return -1;
            }

            if (x.Mean > y.Mean)
            {
                return 1;
            }

            return 0;
        }

        public static bool operator ==(RangedInt x, RangedInt y)
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

        public static bool operator !=(RangedInt x, RangedInt y) => !(x == y);

        public static bool operator >(RangedInt x, RangedInt y)
        {
            if (y is null)
            {
                if (x is null)
                {
                    return false;
                }

                return true;
            }

            return x.Mean > y.Mean;
        }

        public static bool operator <(RangedInt x, RangedInt y)
        {
            if (x is null)
            {
                if (y is null)
                {
                    return false;
                }

                return true;
            }

            return x.Mean < y.Mean;
        }

        public static bool operator >=(RangedInt x, RangedInt y) => x > y || x == y;

        public static bool operator <=(RangedInt x, RangedInt y) => x < y || x == y;
    }
}