using SpaceAce.Main;
using System;

namespace SpaceAce.Auxiliary
{
    public static class AuxMath
    {
        public static float RandomNormal => UnityEngine.Random.Range(-1f, 1f);
        public static float RandomSign => RandomNormal > 0f ? 1f : -1f;
        public static bool RandomBoolean => RandomSign > 0f;

        public static int GetLevelIndex(EnemyType type, LevelDifficulty difficulty)
        {
            int totalDifficulties = Enum.GetValues(typeof(LevelDifficulty)).Length;

            return (int)type * totalDifficulties + (int)difficulty;
        }
    }
}