using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class StasisField : InventoryItem, IEquatable<StasisField>
    {
        public const float MinSlowdown = 0.1f;
        public const float MaxSlowdown = 0.75f;

        private static Coroutine s_stasisField = null;

        [JsonIgnore]
        public override float Worth => (base.Worth + Slowdown * SlowdownUnitWorth) * (float)(Rarity + 1);

        public float Slowdown { get; }

        [JsonIgnore]
        public float SlowdownPercentage => Slowdown * 100f;

        public StasisField(ItemRarity rarity, float duration, float slowdown) : base(rarity, duration)
        {
            Slowdown = Mathf.Clamp(slowdown, MinSlowdown, MaxSlowdown);
        }

        private IEnumerator StasisFieldRoutine(StasisField field)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyStasisField;
            float timer = 0f;

            while (timer < field.Duration)
            {
                if (GameModeLoader.Access.GameMode != GameMode.Level)
                {
                    SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyStasisField;
                    s_stasisField = null;

                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyStasisField;
            s_stasisField = null;
        }

        private void TryApplyStasisField(object receiver)
        {
            if (receiver is IStasisFieldUser user) user.Use(this);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is StasisField other1 &&
                item2 is StasisField other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newSlowdown = (Slowdown + other1.Slowdown + other2.Slowdown) * FusedPropertyFactor;

                result = new StasisField(nextRarity, newDuration, newSlowdown);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == Main.GameMode.Level && s_stasisField == null)
            {
                s_stasisField = CoroutineRunner.RunRoutine(StasisFieldRoutine(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IStasisFieldUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Stasis field", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Stasis field", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Stasis field", "Description");

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

        public override bool Equals(object obj) => Equals(obj as StasisField);

        public bool Equals(StasisField other) => base.Equals(other) && Slowdown.Equals(other.Slowdown);

        public override int GetHashCode() => base.GetHashCode() ^ Slowdown.GetHashCode();
    }
}