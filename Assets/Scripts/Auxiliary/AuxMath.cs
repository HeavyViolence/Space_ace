using UnityEngine;

namespace SpaceAce.Auxiliary
{
    public static class AuxMath
    {
        private const float DegreesPerSecondPerRotationPerMinute = 6f;

        public static float RandomNormal => Random.Range(-1f, 1f);
        public static float RandomSign => RandomNormal > 0f ? 1f : -1f;
        public static bool RandomBoolean => RandomSign > 0f;

        public static float RotationsPerMinuteToDegreesPerSecond(float value) => value * DegreesPerSecondPerRotationPerMinute;
        public static float DegreesPerSecondToRotationsPerMinute(float value) => value / DegreesPerSecondPerRotationPerMinute;
    }
}