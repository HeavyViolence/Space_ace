using UnityEngine;

namespace SpaceAce.Auxiliary
{
    public static class AuxMath
    {
        public static float RandomSign => Random.Range(-1f, 1f) > 0f ? 1f : -1f;
        public static bool RandomBoolean => RandomSign > 0f;
    }
}