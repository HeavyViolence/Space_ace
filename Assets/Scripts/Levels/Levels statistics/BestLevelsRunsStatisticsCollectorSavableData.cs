using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceAce.Levels
{
    [Serializable]
    public sealed class BestLevelsRunsStatisticsCollectorSavableData
    {
        [SerializeField] private List<int> _levelIndices;
        [SerializeField] private List<BestLevelRunStatistics> _statistics;

        public IEnumerable<KeyValuePair<int, BestLevelRunStatistics>> Contents =>
            Enumerable.Zip(_levelIndices, _statistics, (index, stats) => new KeyValuePair<int, BestLevelRunStatistics>());

        public BestLevelsRunsStatisticsCollectorSavableData(IEnumerable<int> levelIndices,
                                                            IEnumerable<BestLevelRunStatistics> statistics)
        {
            _levelIndices = new(levelIndices);
            _statistics = new(statistics);
        }
    }
}