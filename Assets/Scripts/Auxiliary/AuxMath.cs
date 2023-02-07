using UnityEngine;

namespace SpaceAce.Auxiliary
{
    public static class AuxMath
    {
        public static float RandomNormal => Random.Range(-1f, 1f);
        public static float RandomSign => RandomNormal > 0f ? 1f : -1f;
        public static bool RandomBoolean => RandomSign > 0f;
    }
}