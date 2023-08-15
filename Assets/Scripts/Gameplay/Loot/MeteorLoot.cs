using SpaceAce.Gameplay.Inventories;
using System;

namespace SpaceAce.Gameplay.Loot
{
    public sealed class MeteorLoot : Loot, IOreScannerUser
    {
        public bool Use(OreScanner scanner)
        {
            if (scanner is null) throw new ArgumentNullException(nameof(scanner));

            if (SpawnProbabilityIncrease == 0f)
            {
                SpawnProbabilityIncrease = scanner.OreSpawnProbabilityIncrease;
                return true;
            }

            return false;
        }
    }
}