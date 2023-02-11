using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SpaceAce.Main.Saving
{
    [DataContract]
    public sealed class LevelsUnlockerSavableData
    {
        [DataMember]
        public IEnumerable<int> PassedLevelsIndices { get; private set; }

        [DataMember]
        public IEnumerable<int> UnlockedLevelsIndices { get; private set; }

        public LevelsUnlockerSavableData(IEnumerable<int> passedLevelsIndices,
                                         IEnumerable<int> unlockedLevelsIndices)
        {
            if (passedLevelsIndices is null)
            {
                throw new ArgumentNullException(nameof(passedLevelsIndices),
                                                "Attempted to pass an empty passed levels indices collection!");
            }

            PassedLevelsIndices = passedLevelsIndices;

            if (unlockedLevelsIndices is null)
            {
                throw new ArgumentNullException(nameof(unlockedLevelsIndices),
                                                "Attempted to pass an empty unlocked levels indices collection!");
            }
        }
    }
}