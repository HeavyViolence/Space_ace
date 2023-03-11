using System;

namespace SpaceAce.Gameplay.Movement
{
    public interface IEscapable
    {
        event EventHandler Escaped;
        void BeginWatchForEscape(Func<bool> escapeCondition);
    }
}