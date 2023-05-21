using System;

namespace SpaceAce.Gameplay.Movement
{
    public interface IEscapable
    {
        event EventHandler Escaped;

        void StartWatchingForEscape(Func<bool> escapeCondition);
        void StopWatchingForEscape();
    }
}