using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class AdvancedLearning : InventoryItem, IEquatable<AdvancedLearning>
    {
        public const float MinExperienceBoost = 1f;
        public const float MaxExperienceBoost = 10f;

        public const float MinExperienceDepletionSlowdown = 1f;
        public const float MaxExperienceDepletionSlowdown = 10f;

        private static Coroutine s_advancedLearning = null;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth +
                                        ExperienceBoost * ExperienceBoostUnitWorth +
                                        ExperienceDepletionSlowdown * ExperienceDepletionSlowdownUnitWorth) *
                                       (float)(Rarity + 1);

        public float ExperienceBoost { get; }

        public float ExperienceDepletionSlowdown { get; }

        public AdvancedLearning(ItemRarity rarity,
                                float duration,
                                float experienceBoost,
                                float experienceDepletionSlowdown) : base(rarity, duration)
        {
            ExperienceBoost = Mathf.Clamp(experienceBoost, MinExperienceBoost, MaxExperienceBoost);
            ExperienceDepletionSlowdown = Mathf.Clamp(experienceDepletionSlowdown, MinExperienceDepletionSlowdown, MaxExperienceDepletionSlowdown);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item1 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is AdvancedLearning other1 &&
                item2 is AdvancedLearning other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newExperienceBoost = (ExperienceBoost + other1.ExperienceBoost + other2.ExperienceBoost) * FusedPropertyFactor;
                float newExperienceDepletionSlowdown = (ExperienceDepletionSlowdown +
                                                        other1.ExperienceDepletionSlowdown +
                                                        other2.ExperienceDepletionSlowdown) *
                                                       FusedPropertyFactor;

                result = new AdvancedLearning(nextRarity, newDuration, newExperienceBoost, newExperienceDepletionSlowdown);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level && s_advancedLearning == null)
            {
                s_advancedLearning = CoroutineRunner.RunRoutine(ApplyAdvancedLearning(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IAdvancedLearningUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        private IEnumerator ApplyAdvancedLearning(AdvancedLearning learning)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyAdvancedLearning;
            float timer = 0f;

            while (timer < learning.Duration)
            {
                if (GameModeLoader.Access.GameMode != GameMode.Level)
                {
                    SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyAdvancedLearning;
                    s_advancedLearning = null;

                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyAdvancedLearning;
            s_advancedLearning = null;
        }

        private void TryApplyAdvancedLearning(object receiver)
        {
            if (receiver is IAdvancedLearningUser user) user.Use(this);
        }

        public override bool Equals(object obj) => Equals(obj as AdvancedLearning);

        public bool Equals(AdvancedLearning other) => base.Equals(other) &&
                                                      ExperienceBoost.Equals(other.ExperienceBoost) &&
                                                      ExperienceDepletionSlowdown.Equals(other.ExperienceDepletionSlowdown);

        public override int GetHashCode() => base.GetHashCode() ^
                                             ExperienceBoost.GetHashCode() ^
                                             ExperienceDepletionSlowdown.GetHashCode();
    }
}