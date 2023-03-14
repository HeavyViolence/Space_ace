using System;
using System.Collections.Generic;

namespace SpaceAce.Auxiliary
{
    public sealed class RangedFloat : IEquatable<RangedFloat>, IComparable<RangedFloat>, IComparer<RangedFloat>
    {
        public static RangedFloat Zero => new(0f, 0f, 0f, 0f, 0);

        private readonly Random _random;

        public float Pivot { get; }
        public float MinValue { get; }
        public float MaxValue { get; }
        public float RandomValue => (float)_random.NextDouble() * (MaxValue - MinValue) + MinValue;
        public float RangeWidth => MaxValue - MinValue;
        public float Mean => MinValue + RangeWidth / 2f;
        public bool IsZeroed => Equals(Zero);

        public RangedFloat(float pivot, float range)
        {
            if (range < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = pivot - range;
            MaxValue = pivot + range;

            _random = new();
        }

        public RangedFloat(float pivot, float range, int seed)
        {
            if (range < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = pivot - range;
            MaxValue = pivot + range;

            _random = new(seed);
        }

        public RangedFloat(float pivot, float range, float min, float max)
        {
            if (range < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = Math.Clamp(pivot - range, min, float.MaxValue);
            MaxValue = Math.Clamp(pivot + range, float.MinValue, max);

            _random = new();
        }

        public RangedFloat(float pivot, float range, float min, float max, int seed)
        {
            if (range < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Attempted to pass a negative range!");
            }

            Pivot = pivot;
            MinValue = Math.Clamp(pivot - range, min, float.MaxValue);
            MaxValue = Math.Clamp(pivot + range, float.MinValue, max);

            _random = new(seed);
        }

        public override bool Equals(object obj) => Equals(obj as RangedFloat);

        public bool Equals(RangedFloat other) => other is not null && Mean.Equals(other.Mean);

        public override int GetHashCode() => Mean.GetHashCode();

        public int CompareTo(RangedFloat other)
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

        public int Compare(RangedFloat x, RangedFloat y)
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

        public static bool operator ==(RangedFloat x, RangedFloat y)
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

        public static bool operator !=(RangedFloat x, RangedFloat y) => !(x == y);

        public static bool operator >(RangedFloat x, RangedFloat y)
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

        public static bool operator <(RangedFloat x, RangedFloat y)
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

        public static bool operator >=(RangedFloat x, RangedFloat y) => x > y || x == y;

        public static bool operator <=(RangedFloat x, RangedFloat y) => x < y || x == y;
    }
}