using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class PlasmaShield : InventoryItem, IEquatable<PlasmaShield>
    {
        public const float MinArmorBoost = 100f;
        public const float MaxArmorBoost = 10000f;

        public const float MinProjectilesSlowdown = 0.05f;
        public const float MaxProjectilesSlowdown = 0.5f;

        [JsonIgnore]
        public override float Worth => (base.Worth +
                                       ArmorBoost * ArmorUnitWorth +
                                       ProjectilesSlowdown * PlayerSlowdownUnitWorth) *
                                       (float)(Rarity + 1);

        public float ArmorBoost { get; }

        public float ProjectilesSlowdown { get; }

        [JsonIgnore]
        public float ProjectilesSlowdownPercentage => ProjectilesSlowdown * 100f;

        public PlasmaShield(ItemRarity rarity, float duration, float armorBoost, float projectilesSlowdown) : base(rarity, duration)
        {
            ArmorBoost = Mathf.Clamp(armorBoost, MinArmorBoost, MaxArmorBoost);
            ProjectilesSlowdown = Mathf.Clamp01(projectilesSlowdown);
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out IPlasmaShieldUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is PlasmaShield other1 &&
                item2 is PlasmaShield other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration) * FusedPropertyFactor;
                float newArmorBoost = (ArmorBoost + other1.ArmorBoost + other2.ArmorBoost) * FusedPropertyFactor;
                float newProjectilesSlowdown = (ProjectilesSlowdown + other1.ProjectilesSlowdown + other2.ProjectilesSlowdown) * FusedPropertyFactor;

                result = new PlasmaShield(nextRarity, newDuration, newArmorBoost, newProjectilesSlowdown);
                return true;
            }

            result = null;
            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Plasma shield", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Plasma shield", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Plasma shield", "Description");

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

        public override bool Equals(object obj) => Equals(obj as PlasmaShield);

        public bool Equals(PlasmaShield other) => base.Equals(other) &&
                                                  ArmorBoost.Equals(other.ArmorBoost) &&
                                                  ProjectilesSlowdown.Equals(other.ProjectilesSlowdown);

        public override int GetHashCode() => base.GetHashCode() ^ ArmorBoost.GetHashCode() ^ ProjectilesSlowdown.GetHashCode();
    }
}