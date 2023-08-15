using SpaceAce.Gameplay.Inventories;
using System;

namespace SpaceAce.Gameplay.Loot
{
    public sealed class EnemyLoot : Loot, IHardwareScannerUser
    {
        public bool Use(HardwareScanner scanner)
        {
            if (scanner is null) throw new ArgumentNullException(nameof(scanner));

            if (SpawnProbabilityIncrease == 0f)
            {
                SpawnProbabilityIncrease = scanner.HardwareSpawnProbabilityIncrease;
                return true;
            }

            return false;
        }
    }
}