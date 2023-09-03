using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class MeteorRouter : InventoryItem, IEquatable<MeteorRouter>
    {
        public const float MinMeteorSpawnSpeedup = 1f;
        public const float MaxMeteorSpawnSpeedup = 10f;

        [JsonIgnore]
        public override float Worth => (base.Worth + MeteorSpawnSpeedup * SpawnSpeedupUnitWorth) * (float)(Rarity + 1);

        public float MeteorSpawnSpeedup { get; }

        [JsonIgnore]
        public float MeteorSpawnSpeedupPercentage => MeteorSpawnSpeedup * 100f;

        public MeteorRouter(ItemRarity rarity, float duration, float meteorSpawnSpeedup) : base(rarity, duration)
        {
            MeteorSpawnSpeedup = Mathf.Clamp(meteorSpawnSpeedup, MinMeteorSpawnSpeedup, MaxMeteorSpawnSpeedup);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is MeteorRouter other1 &&
                item2 is MeteorRouter other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newMeteorSpawnSpeedup = (MeteorSpawnSpeedup +
                                               other1.MeteorSpawnSpeedup +
                                               other2.MeteorSpawnSpeedup) *
                                              FusedPropertyFactor;

                result = new MeteorRouter(nextRarity, newDuration, newMeteorSpawnSpeedup);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out IMeteorRouterUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Meteor router", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Meteor router", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Meteor router", "Description");

            var titleOperation = title.GetLocalizedStringAsync();
            await titleOperation;
            string localizedTitle = titleOperation.Result;

            var rarityOperation = rarity.GetLocalizedStringAsync();
            await rarityOperation;
            string localizedRarity = rarityOperation.Result;

            var statsOperation = stats.GetLocalizedStringAsync();
            await statsOperation;
            string localizedStats = statsOperation.Result;

            var descriptionOperation = description.GetLocalizedStringAsync();
            await descriptionOperation;
            string localizedDescription = descriptionOperation.Result;

            return $"{localizedTitle}\n{localizedRarity}\n\n{localizedStats}\n\n{localizedDescription}";
        }

        public override bool Equals(object obj) => Equals(obj as MeteorRouter);

        public bool Equals(MeteorRouter other) => base.Equals(other) && MeteorSpawnSpeedup.Equals(other.MeteorSpawnSpeedup);

        public override int GetHashCode() => base.GetHashCode() ^ MeteorSpawnSpeedup.GetHashCode();
    }
}