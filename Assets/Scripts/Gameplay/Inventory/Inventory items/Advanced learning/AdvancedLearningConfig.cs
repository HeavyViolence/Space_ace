using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    [CreateAssetMenu(fileName = "Advanced learning", menuName = "Space ace/Configs/Loot/Advanced learning")]
    public sealed class AdvancedLearningConfig : InventoryItemConfig
    {
        [SerializeField] private float _experienceBoost = AdvancedLearning.MinExperienceBoost;
        [SerializeField] private float _experienceBoostRandomDeviation = 0f;

        [SerializeField] private float _experienceDepletionSlowdown = AdvancedLearning.MinExperienceDepletionSlowdown;
        [SerializeField] private float _experienceDepletionSlowdownRandomDeviation = 0f;

        public RangedFloat ExperienceBoost { get; private set; }

        public RangedFloat ExperienceDepletionSlowdown { get; private set; }

        public override void ApplySettings()
        {
            base.ApplySettings();

            ExperienceBoost = new(_experienceBoost,
                                  _experienceBoostRandomDeviation,
                                  AdvancedLearning.MinExperienceBoost,
                                  AdvancedLearning.MaxExperienceBoost);

            ExperienceDepletionSlowdown = new(_experienceDepletionSlowdown,
                                              _experienceDepletionSlowdownRandomDeviation,
                                              AdvancedLearning.MinExperienceDepletionSlowdown,
                                              AdvancedLearning.MaxExperienceDepletionSlowdown);
        }

        public override InventoryItem GetItem() => new AdvancedLearning(Rarity,
                                                                        Duration.RandomValue,
                                                                        ExperienceBoost.RandomValue,
                                                                        ExperienceDepletionSlowdown.RandomValue);
    }
}