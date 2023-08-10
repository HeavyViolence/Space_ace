using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class ArmorDiffuser : InventoryItem, IEquatable<ArmorDiffuser>
    {
        public const float MinArmorReduction = 10f;
        public const float MaxArmorReduction = 1000f;

        private static Coroutine s_armorDiffuser = null;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + ArmorReduction * ArmorUnitWorth) * (float)(Rarity + 1);

        public float ArmorReduction { get; private set; }

        public ArmorDiffuser(ItemRarity rarity, float duration, float armorReduction) : base(rarity, duration)
        {
            ArmorReduction = Mathf.Clamp(armorReduction, MinArmorReduction, MaxArmorReduction);
        }

        private IEnumerator ArmorDiffuserRoutine(ArmorDiffuser diffuser)
        {
            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate += TryApplyArmorDiffuser;
            float timer = 0f;

            while (timer < diffuser.Duration)
            {
                timer += Time.deltaTime;

                if (GameModeLoader.Access.GameState != GameState.Level) yield break;

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
            if (GameModeLoader.Access.GameState == GameState.Level && s_armorDiffuser == null)
            {
                s_armorDiffuser = CoroutineRunner.RunRoutine(ArmorDiffuserRoutine(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IArmorDiffuserUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as ArmorDiffuser);

        public bool Equals(ArmorDiffuser other) => base.Equals(other) && ArmorReduction.Equals(other.ArmorReduction);

        public override int GetHashCode() => base.GetHashCode() ^ ArmorReduction.GetHashCode();
    }
}