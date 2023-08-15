using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Atomizer : InventoryItem, IEquatable<Atomizer>
    {
        public const int MinEntitiesToBeDestroyed = 1;
        public const int MaxEntitiesToBeDestroyed = 100;

        private static Coroutine s_atomizer = null;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public float EntityDestructionDelay => Duration / EntitiesToBeDestroyed;

        [JsonIgnore]
        public override float Worth => (base.Worth + EntitiesToBeDestroyed * KillWorth) * (float)(Rarity + 1);

        public int EntitiesToBeDestroyed { get; }

        private static IEnumerator ApplyAtomizer(Atomizer atomizer)
        {
            float timer;

            for (int i = 0; i < atomizer.EntitiesToBeDestroyed; i++)
            {
                if (GameModeLoader.Access.GameState != GameState.Level)
                {
                    s_atomizer = null;
                    yield break;
                }

                timer = 0f;

                while (timer < atomizer.EntityDestructionDelay)
                {
                    timer += Time.deltaTime;

                    yield return null;
                    while (GamePauser.Access.Paused == true) yield return null;
                }

                if (SpecialEffectsMediator.Access.TryGetFirstEffectReceiver(out IAtomizerUser user)) user.Use(atomizer);
            }

            s_atomizer = null;
        }

        public Atomizer(ItemRarity rarity, float duration, int entitiesToBeDestroyed) : base(rarity, duration)
        {
            EntitiesToBeDestroyed = Mathf.Clamp(entitiesToBeDestroyed, MinEntitiesToBeDestroyed, MaxEntitiesToBeDestroyed);
        }

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is Atomizer other1 &&
                item2 is Atomizer other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                int newEntitiesToBeDestroyedAmount = (int)((EntitiesToBeDestroyed +
                                                            other1.EntitiesToBeDestroyed +
                                                            other2.EntitiesToBeDestroyed) *
                                                           FusedPropertyFactor);

                result = new Atomizer(nextRarity, newDuration, newEntitiesToBeDestroyedAmount);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == GameState.Level && s_atomizer == null)
            {
                s_atomizer = CoroutineRunner.RunRoutine(ApplyAtomizer(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as Atomizer);

        public bool Equals(Atomizer other) => base.Equals(other) && EntitiesToBeDestroyed.Equals(other.EntitiesToBeDestroyed);

        public override int GetHashCode() => base.GetHashCode() ^ EntitiesToBeDestroyed.GetHashCode();
    }
}