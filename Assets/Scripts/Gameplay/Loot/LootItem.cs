using SpaceAce.Gameplay.Inventory;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Main.Audio;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Loot
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(LootItemMovement))]
    public sealed class LootItem : MonoBehaviour
    {
        [SerializeField] private AudioCollection _lootCollectionAudio;

        public event EventHandler Collected;

        private InventoryItem _lootItem;
        private SpriteRenderer _lootIconRenderer;
        private Animator _lootItemAnimator;

        private void Awake()
        {
            _lootIconRenderer = transform.GetComponent<SpriteRenderer>();

            if (transform.TryGetComponent(out Animator animator) == true)
            {
                _lootItemAnimator = animator;
            }
            else
            {
                throw new MissingComponentException($"Loot item {name} doesn't have a mandatory component of type {typeof(Animator)}!");
            }
        }

        private void OnDisable()
        {
            _lootItem = null;
            _lootIconRenderer = null;
            Collected = null;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            // Implement player inventory to proceed here on loot pickup

            _lootCollectionAudio.PlayRandomAudioClip(Vector2.zero);
            Collected?.Invoke(this, EventArgs.Empty);
        }

        public void SetContent(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), $"Attempted to pass an empty inventory item as a loot item!");

            _lootItem = item;
            _lootIconRenderer.sprite = item.Icon;

            switch (item.Rarity)
            {
                case ItemRarity.Common:
                    {
                        _lootItemAnimator.SetTrigger("Common");
                        break;
                    }
                case ItemRarity.Uncommon:
                    {
                        _lootItemAnimator.SetTrigger("Uncommon");
                        break;
                    }
                case ItemRarity.Rare:
                    {
                        _lootItemAnimator.SetTrigger("Rare");
                        break;
                    }
                case ItemRarity.Exceptional:
                    {
                        _lootItemAnimator.SetTrigger("Exceptional");
                        break;
                    }
                case ItemRarity.Exotic:
                    {
                        _lootItemAnimator.SetTrigger("Exotic");
                        break;
                    }
                case ItemRarity.Epic:
                    {
                        _lootItemAnimator.SetTrigger("Epic");
                        break;
                    }
                case ItemRarity.Legendary:
                    {
                        _lootItemAnimator.SetTrigger("Legendary");
                        break;
                    }
            }
        }

        public InventoryItem GetContent() => _lootItem;
    }
}