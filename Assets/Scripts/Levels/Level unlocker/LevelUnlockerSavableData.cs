using System;
using System.Collections.Generic;

namespace SpaceAce.Levels
{
    public sealed class LevelUnlockerSavableData
    {
        public HashSet<int> PassedLevels { get; }
        public HashSet<int> UnlockedLevels { get; }

        public LevelUnlockerSavableData(IEnumerable<int> passedLevels, IEnumerable<int> unlockedLevels)
        {
            if (passedLevels is null) throw new ArgumentNullException(nameof(passedLevels));
            PassedLevels = new(passedLevels);

            if (unlockedLevels is null) throw new ArgumentNullException(nameof(unlockedLevels));
            UnlockedLevels = new(unlockedLevels);
        }
    }
}