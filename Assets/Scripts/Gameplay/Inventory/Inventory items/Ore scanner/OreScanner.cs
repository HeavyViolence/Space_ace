using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class OreScanner : InventoryItem, IEquatable<OreScanner>
    {
        public const float MinOreSpawnProbabilityIncrease = 0f;
        public const float MaxOreSpawnProbabilityIncrease = 1f;

        private static Coroutine s_oreScanner = null;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + OreSpawnProbabilityIncrease * ItemSpawnProbabilityUnitWorth) * (float)(Rarity + 1);

        public float OreSpawnProbabilityIncrease { get; }

        public OreScanner(ItemRarity rarity, float duration, float oreSpawnProbabilityIncrease) : base(rarity, duration)
        {
            OreSpawnProbabilityIncrease = Mathf.Clamp(oreSpawnProbabilityIncrease,
                                                      MinOreSpawnProbabilityIncrease,
                                                      MaxOreSpawnProbabilityIncrease);
        }

        private IEnumerator ApplyOreScanner(OreScanner scanner)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyOreScanner;
            float timer = 0f;

            while (timer < scanner.Duration)
            {
                if (GameModeLoader.Access.GameState != GameState.Level)
                {
                    SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyOreScanner;
                    s_oreScanner = null;

                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyOreScanner;
            s_oreScanner = null;
        }

        private void TryApplyOreScanner(object receiver)
        {
            if (receiver is IOreScannerUser user) user.Use(this);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is OreScanner other1 &&
                item2 is OreScanner other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newOreSpawnProbabilityIncrease = (OreSpawnProbabilityIncrease +
                                                        other1.OreSpawnProbabilityIncrease +
                                                        other2.OreSpawnProbabilityIncrease) *
                                                       FusedPropertyFactor;

                result = new OreScanner(nextRarity, newDuration, newOreSpawnProbabilityIncrease);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == GameState.Level && s_oreScanner == null)
            {
                s_oreScanner = CoroutineRunner.RunRoutine(ApplyOreScanner(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IOreScannerUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as OreScanner);

        public bool Equals(OreScanner other) => base.Equals(other) &&
                                                OreSpawnProbabilityIncrease.Equals(other.OreSpawnProbabilityIncrease);

        public override int GetHashCode() => base.GetHashCode() ^ OreSpawnProbabilityIncrease.GetHashCode();
    }
}