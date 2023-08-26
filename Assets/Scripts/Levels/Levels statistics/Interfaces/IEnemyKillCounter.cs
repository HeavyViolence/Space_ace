using System;

namespace SpaceAce.Levels
{
    public interface IEnemyKillCounter
    {
        event EventHandler EnemyKilled;
    }
}