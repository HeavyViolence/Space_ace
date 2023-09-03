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
    public sealed class EMP : InventoryItem, IEquatable<EMP>
    {
        public const float MinJamProbability = 0f;
        public const float MaxJamProbability = 1f;

        private static Coroutine s_EMP = null;

        [JsonIgnore]
        public override float Worth => (base.Worth + JamProbability * JamProbabilityUnitWorth) * (float)(Rarity + 1);

        public float JamProbability { get; }

        [JsonIgnore]
        public float JamProbabilityPercentage => JamProbability * 100f;

        public EMP(ItemRarity rarity, float duration, float jamProbability) : base(rarity, duration)
        {
            JamProbability = Mathf.Clamp(jamProbability, MinJamProbability, MaxJamProbability);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is EMP other1 &&
                item2 is EMP other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newJamProbability = (JamProbability + other1.JamProbability + other2.JamProbability) * FusedPropertyFactor;

                result = new EMP(nextRarity, newDuration, newJamProbability);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level && s_EMP == null)
            {
                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IEMPUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                s_EMP = CoroutineRunner.RunRoutine(ApplyEMP(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("EMP", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("EMP", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("EMP", "Description");

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

        private IEnumerator ApplyEMP(EMP emp)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyEMP;
            float timer = 0f;

            while (timer < emp.Duration)
            {
                if (GameModeLoader.Access.GameMode != GameMode.Level)
                {
                    SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyEMP;
                    s_EMP = null;

                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyEMP;
            s_EMP = null;
        }

        private void TryApplyEMP(object receiver)
        {
            if (receiver is IEMPUser user) user.Use(this);
        }

        public override bool Equals(object obj) => Equals(obj as EMP);

        public bool Equals(EMP other) => base.Equals(other) && JamProbability.Equals(other.JamProbability);

        public override int GetHashCode() => base.GetHashCode() ^ JamProbability.GetHashCode();
    }
}