using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class Atomizer : InventoryItem, IEquatable<Atomizer>
    {
        public const int MinEntitiesToBeDestroyed = 1;
        public const int MaxEntitiesToBeDestroyed = 100;

        private static Coroutine s_atomizer = null;

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
                if (GameModeLoader.Access.GameMode != GameMode.Level)
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
            if (GameModeLoader.Access.GameMode == GameMode.Level && s_atomizer == null)
            {
                s_atomizer = CoroutineRunner.RunRoutine(ApplyAtomizer(this));
                HUDDisplay.Access.RegisterActiveItem(this);

                return true;
            }

            return false;
        }

        public override async UniTask<string> GetDescription()
        {
            LocalizedString title = new("Atomizer", "Title");
            LocalizedString rarity = new("Rarity", Rarity.ToString());
            LocalizedString stats = new("Atomizer", "Stats") { Arguments = new[] { this } };
            LocalizedString description = new("Atomizer", "Description");

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

        public override bool Equals(object obj) => Equals(obj as Atomizer);

        public bool Equals(Atomizer other) => base.Equals(other) && EntitiesToBeDestroyed.Equals(other.EntitiesToBeDestroyed);

        public override int GetHashCode() => base.GetHashCode() ^ EntitiesToBeDestroyed.GetHashCode();
    }
}