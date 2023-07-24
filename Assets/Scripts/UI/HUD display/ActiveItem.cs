using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Inventories;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class ActiveItem : IEquatable<ActiveItem>
    {
        private readonly Label _timerLabel;

        public float Timer { get; private set; }
        public bool TimerIsUp => Timer < 0f;
        public InventoryItem Item { get; }
        public VisualElement Thumbnail { get; }

        public ActiveItem(InventoryItem item, VisualElement thumbnail)
        {
            Item = item;
            Timer = item.Duration;
            Thumbnail = thumbnail;

            _timerLabel = Thumbnail.Q<Label>("Duration-label");
            _timerLabel.text = AuxMath.GetFormattedTime((int)Timer);

            var icon = Thumbnail.Q<VisualElement>("Icon");
            icon.style.backgroundColor = new(Item.RarityColor);
            icon.style.backgroundImage = Item.Icon.texture;
        }

        public void Tick()
        {
            Timer -= Time.deltaTime;
            _timerLabel.text = AuxMath.GetFormattedTime((int)Timer);
        }

        public override bool Equals(object obj) => Equals(obj as ActiveItem);

        public bool Equals(ActiveItem other) => other is not null && Item.Equals(other.Item);

        public override int GetHashCode() => Item.GetHashCode();
    }
}