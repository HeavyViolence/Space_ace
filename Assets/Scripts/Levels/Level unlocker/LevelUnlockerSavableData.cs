using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Levels
{
    [Serializable]
    public sealed class LevelUnlockerSavableData
    {
        [SerializeField] private List<int> _passedLevels;
        [SerializeField] private List<int> _unlockedLevels;

        public IEnumerable<int> PassedLevels => _passedLevels;
        public IEnumerable<int> UnlockedLevels => _unlockedLevels;

        public LevelUnlockerSavableData(IEnumerable<int> passedLevels,
                                        IEnumerable<int> unlockedLevels)
        {
            if (passedLevels is null)
            {
                throw new ArgumentNullException(nameof(passedLevels),
                                                "Attempted to pass an empty passed levels indices collection!");
            }

            _passedLevels = new(passedLevels);

            if (unlockedLevels is null)
            {
                throw new ArgumentNullException(nameof(unlockedLevels),
                                                "Attempted to pass an empty unlocked levels indices collection!");
            }

            _unlockedLevels = new(unlockedLevels);
        }
    }
}