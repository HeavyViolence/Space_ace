using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Gameplay.Players;
using SpaceAce.Main;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    public sealed class InventoryDisplay : UIDisplay
    {
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();
        private static readonly GameServiceFastAccess<Player> s_player = new();
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        private readonly VisualTreeAsset _inventorySlot;
        private readonly GameControls _gameControls = new();
        private readonly ItemInfuser _itemInfuser = new();

        private VisualElement _inventoryContainer;
        private Button _backButton;
        private Button _infuseItemsButton;

        public override string DisplayHolderName => "Inventory display";

        public InventoryDisplay(UIAssets assets) : base(assets.InventoryMenu, assets.Settings, assets.UIAudio)
        {
            _inventorySlot = assets.InventorySLot;
        }

        public override void OnInitialize()
        {
            GameServices.Register(this);
        }

        public override void OnSubscribe()
        {
            _itemInfuser.ItemToInfuseReleased += ItemToInfuseReleasedEventHandler;
            _itemInfuser.InfusedItemReleased += InfusedItemReleasedEventHandler;
        }

        public override void OnUnsubscribe()
        {
            _itemInfuser.ItemToInfuseReleased -= ItemToInfuseReleasedEventHandler;
            _itemInfuser.InfusedItemReleased -= InfusedItemReleasedEventHandler;
        }

        public override void OnClear()
        {
            GameServices.Deregister(this);
        }

        public override void Enable()
        {
            base.Enable();

            if (s_gameModeLoader.Access.GameState == GameState.Level) s_gamePauser.Access.Pause();

            DisplayedDocument.visualTreeAsset = Display;

            _backButton = DisplayedDocument.rootVisualElement.Q<Button>("Back-button");
            _backButton.clicked += BackButtonClickedEventHandler;

            _infuseItemsButton = DisplayedDocument.rootVisualElement.Q<Button>("Infuse-button");
            _infuseItemsButton.clicked += InfuseItemsButtonClickedEventHandler;

            _itemInfuser.SetInventoryDisplay(DisplayedDocument.rootVisualElement);
            _itemInfuser.OnEnable();

            _gameControls.Menu.Enable();
            _gameControls.Menu.Back.performed += (c) => BackButtonClickedEventHandler();
            _gameControls.Menu.Inventory.performed += (c) => BackButtonClickedEventHandler();

            _inventoryContainer = DisplayedDocument.rootVisualElement.Q<VisualElement>("Inventory-container");

            s_player.Access.Inventory.ContentChanged += (s, e) => UpdateInventoryView();

            UpdateInventoryView();
        }

        protected override void Disable()
        {
            base.Disable();

            _itemInfuser.OnDisable();

            if (_itemInfuser.TryReclaimItemsToInfuse(out var items) == true) s_player.Access.Inventory.AddItems(items);
            if (_itemInfuser.TryCollectInfusedItem(out InventoryItem item) == true) s_player.Access.Inventory.AddItem(item);

            _gameControls.Menu.Disable();
            _gameControls.Menu.Back.performed -= (c) => BackButtonClickedEventHandler();
            _gameControls.Menu.Inventory.performed -= (c) => BackButtonClickedEventHandler();

            _backButton.clicked -= BackButtonClickedEventHandler;
            _infuseItemsButton.clicked -= InfuseItemsButtonClickedEventHandler;

            DisplayedDocument.visualTreeAsset = null;

            s_player.Access.Inventory.ContentChanged -= (s, e) => UpdateInventoryView();
        }

        private void UpdateInventoryView()
        {
            if (_inventoryContainer.childCount > 0) _inventoryContainer.Clear();

            int counter = 0;

            foreach (var item in s_player.Access.Inventory.GetContent())
            {
                var inventorySlot = _inventorySlot.CloneTree();
                inventorySlot.name = $"Inventory-slot-{counter++}";

                var useItemButton = inventorySlot.Q<Button>("Use-item-button");
                useItemButton.clicked += () => UseItem(item);

                if (s_gameModeLoader.Access.GameState == GameState.Level) useItemButton.SetEnabled(true);
                else useItemButton.SetEnabled(false);

                var sellItemButton = inventorySlot.Q<Button>("Sell-item-button");
                sellItemButton.clicked += () => SellItem(item);

                if (s_gameModeLoader.Access.GameState == GameState.MainMenu) sellItemButton.SetEnabled(true);
                else sellItemButton.SetEnabled(false);

                var fuseItemButton = inventorySlot.Q<Button>("Fuse-item-button");
                fuseItemButton.clicked += () => AddItemToInfuse(item);

                if (s_gameModeLoader.Access.GameState == GameState.MainMenu) fuseItemButton.SetEnabled(true);
                else fuseItemButton.SetEnabled(false);

                var itemIcon = inventorySlot.Q<VisualElement>("Item-icon");
                itemIcon.style.backgroundColor = new(item.RarityColor);
                itemIcon.style.backgroundImage = new(item.Icon);

                _inventoryContainer.Add(inventorySlot);
            }
        }

        #region event handlers

        private void BackButtonClickedEventHandler()
        {
            Disable();
            UIAudio.BackButtonClick.PlayRandomAudioClip(Vector2.zero);

            if (s_gameModeLoader.Access.GameState == GameState.Level)
            {
                if (GameServices.TryGetService<HUDDisplay>(out var display) == true) display.Enable();
                else throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
            }
            else
            {
                if (GameServices.TryGetService<MainMenuDisplay>(out var display) == true) display.Enable();
                else throw new UnregisteredGameServiceAccessAttemptException(typeof(MainMenuDisplay));
            }
        }

        private void InfuseItemsButtonClickedEventHandler()
        {
            if (_itemInfuser.TryInfuseItems() == true)
            {
                UIAudio.GainedOrUnlocked.PlayRandomAudioClip(Vector2.zero);
                _infuseItemsButton.SetEnabled(false);
            }
            else
            {
                UIAudio.Error.PlayRandomAudioClip(Vector2.zero);
            }
        }

        private void UseItem(InventoryItem item)
        {
            if (item.Use() == true)
            {
                s_player.Access.Inventory.RemoveItem(item);
                UIAudio.Powerup.PlayRandomAudioClip(Vector2.zero);
            }
            else
            {
                UIAudio.Error.PlayRandomAudioClip(Vector2.zero);
            }
        }

        private void SellItem(InventoryItem item)
        {
            s_player.Access.Inventory.RemoveItem(item);
            s_player.Access.Wallet.AddCredits((int)item.Worth);
            UIAudio.ItemSold.PlayRandomAudioClip(Vector2.zero);
        }

        private void AddItemToInfuse(InventoryItem item)
        {
            if (_itemInfuser.TryAddItemToInfuse(item) == true)
            {
                s_player.Access.Inventory.RemoveItem(item);
                UIAudio.ForwardButtonClick.PlayRandomAudioClip(Vector2.zero);

                if (_itemInfuser.InfusionSlotsPopulated &&
                    _itemInfuser.InfusedItemSlotOccupied == false) _infuseItemsButton.SetEnabled(true);
            }
            else
            {
                UIAudio.Error.PlayRandomAudioClip(Vector2.zero);
            }
        }

        private void ItemToInfuseReleasedEventHandler(object sender, ItemEventArgs e)
        {
            s_player.Access.Inventory.AddItem(e.Item);
            _infuseItemsButton?.SetEnabled(false);
            UIAudio.ForwardButtonClick.PlayRandomAudioClip(Vector2.zero);
        }

        private void InfusedItemReleasedEventHandler(object sender, ItemEventArgs e)
        {
            s_player.Access.Inventory.AddItem(e.Item);
            UIAudio.ForwardButtonClick.PlayRandomAudioClip(Vector2.zero);

            if (_itemInfuser.InfusionSlotsPopulated) _infuseItemsButton.SetEnabled(true);
        }

        #endregion
    }
}