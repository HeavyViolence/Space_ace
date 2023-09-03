using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Main;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Sponsorship : InventoryItem, IEquatable<Sponsorship>
    {
        public const float MinExperienceToCreditsConversionRate = 0f;
        public const float MaxExperienceToCreditsConversionRate = 1f;

        [JsonIgnore]
        public override float Worth => (base.Worth + ExperienceToCreditsConversionRate * ExperienceToCreditsConversionRateUnitWorth) * (float)(Rarity + 1);

        public float ExperienceToCreditsConversionRate { get; }

        [JsonIgnore]
        public float ExperienceToCreditsConversionRatePercentage => ExperienceToCreditsConversionRate * 100f;

        public Sponsorship(ItemRarity rarity,
                           float duration,
                           float experienceToCreditsConversionRate) : base(rarity, duration)
        {
            ExperienceToCreditsConversionRate = Mathf.Clamp(experienceToCreditsConversionRate,
                                                            MinExperienceToCreditsConversionRate,
                                                            MaxExperienceToCreditsConversionRate);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is Sponsorship other1 &&
                item2 is Sponsorship other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newExperienceToCreditsConversionRate = (ExperienceToCreditsConversionRate +
                                                              other1.ExperienceToCreditsConversionRate +
                                                              other2.ExperienceToCreditsConversionRate) *
                                                             FusedPropertyFactor;

                result = new Sponsorship(nextRarity, newDuration, newExperienceToCreditsConversionRate);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level &&
                SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out ISponsorshipUser user) == true &&
                user.Use(this) == true)
            {
                HUDDisplay.Access.RegisterActiveItem(this);
                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Sponsorship", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Sponsorship", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Sponsorship", "Description");

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

        public override bool Equals(object obj) => Equals(obj as Sponsorship);

        public bool Equals(Sponsorship other) => base.Equals(other) &&
                                                 ExperienceToCreditsConversionRate.Equals(other.ExperienceToCreditsConversionRate);

        public override int GetHashCode() => base.GetHashCode() ^ ExperienceToCreditsConversionRate.GetHashCode();
    }
}