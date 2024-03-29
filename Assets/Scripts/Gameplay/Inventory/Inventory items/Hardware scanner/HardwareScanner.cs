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
    public sealed class HardwareScanner : InventoryItem, IEquatable<HardwareScanner>
    {
        public const float MinHardawareSpawnProbabilityIncrease = 0f;
        public const float MaxHardwareSpawnProbabilityIncrease = 1f;

        private static Coroutine s_hardwareScanner = null;

        [JsonIgnore]
        public override float Worth => (base.Worth + HardwareSpawnProbabilityIncrease * ItemSpawnProbabilityUnitWorth) * (float)(Rarity + 1);

        public float HardwareSpawnProbabilityIncrease { get; }

        [JsonIgnore]
        public float HardwareSpawnProbabilityIncreasePercentage => HardwareSpawnProbabilityIncrease * 100f;

        public HardwareScanner(ItemRarity rarity, float duration, float hardwareSpawnProbabilityIncrease) : base(rarity, duration)
        {
            HardwareSpawnProbabilityIncrease = Mathf.Clamp(hardwareSpawnProbabilityIncrease,
                                                           MinHardawareSpawnProbabilityIncrease,
                                                           MaxHardwareSpawnProbabilityIncrease);
        }

        private IEnumerator ApplyHardwareScanner(HardwareScanner scanner)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyHardwareScanner;
            float timer = 0f;

            while (timer < scanner.Duration)
            {
                if (GameModeLoader.Access.GameMode != GameMode.Level)
                {
                    SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyHardwareScanner;
                    s_hardwareScanner = null;

                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyHardwareScanner;
            s_hardwareScanner = null;
        }

        private void TryApplyHardwareScanner(object receiver)
        {
            if (receiver is IHardwareScannerUser user) user.Use(this);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is HardwareScanner other1 &&
                item2 is HardwareScanner other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newHardwareSpawnProbabilityIncrease = (HardwareSpawnProbabilityIncrease +
                                                             other1.HardwareSpawnProbabilityIncrease +
                                                             other2.HardwareSpawnProbabilityIncrease) *
                                                            FusedPropertyFactor;

                result = new HardwareScanner(nextRarity, newDuration, newHardwareSpawnProbabilityIncrease);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level && s_hardwareScanner == null)
            {
                s_hardwareScanner = CoroutineRunner.RunRoutine(ApplyHardwareScanner(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IHardwareScannerUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Hardware scanner", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Hardware scanner", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Hardware scanner", "Description");

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

        public override bool Equals(object obj) => Equals(obj as HardwareScanner);

        public bool Equals(HardwareScanner other) => base.Equals(other) &&
                                                     HardwareSpawnProbabilityIncrease.Equals(other.HardwareSpawnProbabilityIncrease);

        public override int GetHashCode() => base.GetHashCode() ^ HardwareSpawnProbabilityIncrease.GetHashCode();
    }
}