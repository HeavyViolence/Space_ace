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
    public sealed class ArmorDiffuser : InventoryItem, IEquatable<ArmorDiffuser>
    {
        public const float MinArmorReduction = 10f;
        public const float MaxArmorReduction = 1000f;

        private static Coroutine s_armorDiffuser = null;

        [JsonIgnore]
        public override float Worth => (base.Worth + ArmorReduction * ArmorUnitWorth) * (float)(Rarity + 1);

        public float ArmorReduction { get; }

        public ArmorDiffuser(ItemRarity rarity, float duration, float armorReduction) : base(rarity, duration)
        {
            ArmorReduction = Mathf.Clamp(armorReduction, MinArmorReduction, MaxArmorReduction);
        }

        private IEnumerator ApplyArmorDiffuser(ArmorDiffuser diffuser)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyArmorDiffuser;
            float timer = 0f;

            while (timer < diffuser.Duration)
            {
                if (GameModeLoader.Access.GameMode != GameMode.Level)
                {
                    SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyArmorDiffuser;
                    s_armorDiffuser = null;

                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyArmorDiffuser;
            s_armorDiffuser = null;
        }

        private void TryApplyArmorDiffuser(object receiver)
        {
            if (receiver is IArmorDiffuserUser user) user.Use(this);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is ArmorDiffuser other1 &&
                item2 is ArmorDiffuser other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newArmorReduction = (ArmorReduction + other1.ArmorReduction + other2.ArmorReduction) * FusedPropertyFactor;

                result = new ArmorDiffuser(nextRarity, newDuration, newArmorReduction);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameMode == GameMode.Level && s_armorDiffuser == null)
            {
                s_armorDiffuser = CoroutineRunner.RunRoutine(ApplyArmorDiffuser(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IArmorDiffuserUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Armor diffuser", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Armor diffuser", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Armor diffuser", "Description");

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

        public override bool Equals(object obj) => Equals(obj as ArmorDiffuser);

        public bool Equals(ArmorDiffuser other) => base.Equals(other) && ArmorReduction.Equals(other.ArmorReduction);

        public override int GetHashCode() => base.GetHashCode() ^ ArmorReduction.GetHashCode();
    }
}