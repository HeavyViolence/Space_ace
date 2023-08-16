using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Sponsorship", menuName = "Space ace/Configs/Loot/Sponsorship")]
    public sealed class SponsorshipConfig : InventoryItemConfig
    {
        [SerializeField] private float _experienceToCreditsConversionRate = Sponsorship.MinExperienceToCreditsConversionRate;
        [SerializeField] private float _experienceToCreditsConversionRateRandomDeviation = 0f;

        public RangedFloat ExperienceToCreditsConversionRate { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            ExperienceToCreditsConversionRate = new(_experienceToCreditsConversionRate,
                                                    _experienceToCreditsConversionRateRandomDeviation,
                                                    Sponsorship.MinExperienceToCreditsConversionRate,
                                                    Sponsorship.MaxExperienceToCreditsConversionRate);
        }

        public override InventoryItem GetItem() => new Sponsorship(Rarity,
                                                                   Duration.RandomValue,
                                                                   ExperienceToCreditsConversionRate.RandomValue);
    }
}