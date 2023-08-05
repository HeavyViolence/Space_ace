using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class StasisField : InventoryItem, IEquatable<StasisField>
    {
        public const float MinSlowdown = 0.1f;
        public const float MaxSlowdown = 0.75f;

        private static Coroutine s_stasisFieldRoutine = null;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + Slowdown * SlowdownUnitWorth) * (float)(Rarity + 1);

        public float Slowdown { get; private set; }

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
                timer += Time.deltaTime;

                if (GameModeLoader.Access.GameState != GameState.Level) yield break;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            SpecialEffectsMediator.Access.RegisteredReceiverBehaviourUpdate -= TryApplyStasisField;
            s_stasisFieldRoutine = null;
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
            if (GameModeLoader.Access.GameState == Main.GameState.Level && s_stasisFieldRoutine == null)
            {
                s_stasisFieldRoutine = CoroutineRunner.RunRoutine(StasisFieldRoutine(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                if (SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IStasisFieldUser> users) == true)
                {
                    foreach (var user in users) user.Use(this);
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as StasisField);

        public bool Equals(StasisField other) => base.Equals(other) && Slowdown.Equals(other.Slowdown);

        public override int GetHashCode() => base.GetHashCode() ^ Slowdown.GetHashCode();
    }
}