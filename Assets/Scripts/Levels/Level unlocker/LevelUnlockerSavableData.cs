using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SpaceAce.Levels
{
    [DataContract]
    public sealed class LevelUnlockerSavableData
    {
        [DataMember]
        public IEnumerable<int> PassedLevels { get; private set; }

        [DataMember]
        public IEnumerable<int> UnlockedLevels { get; private set; }

        public LevelUnlockerSavableData(IEnumerable<int> passedLevels,
                                        IEnumerable<int> unlockedLevels)
        {
            if (passedLevels is null)
            {
                throw new ArgumentNullException(nameof(passedLevels),
                                                "Attempted to pass an empty passed levels indices collection!");
            }

            PassedLevels = passedLevels;

            if (unlockedLevels is null)
            {
                throw new ArgumentNullException(nameof(unlockedLevels),
                                                "Attempted to pass an empty unlocked levels indices collection!");
            }
        }
    }
}