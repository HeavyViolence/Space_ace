using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Gameplay.Players;
using SpaceAce.Main.Audio;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Loot
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(LootItemMovement))]
    public sealed class LootItem : MonoBehaviour
    {
        private static readonly GameServiceFastAccess<Player> s_player = new();

        public event EventHandler Collected;

        [SerializeField] private AudioCollection _lootCollectionAudio;

        private InventoryItem _content;
        private SpriteRenderer _contentIconRenderer;
        private Animator _lootItemAnimator;

        private void Awake()
        {
            _contentIconRenderer = transform.GetComponent<SpriteRenderer>();

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
            _content = null;
            Collected = null;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            s_player.Access.Inventory.AddItem(_content);
            _lootCollectionAudio.PlayRandomAudioClip(Vector2.zero);

            Collected?.Invoke(this, EventArgs.Empty);
        }

        public void SetContent(InventoryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), $"Attempted to pass an empty inventory item as a loot item!");

            _content = item;
            _contentIconRenderer.sprite = item.Icon;

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
    }
}