using SpaceAce.Architecture;
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

        private readonly VisualTreeAsset _inventorySlot;
        private readonly GameControls _gameControls;

        public override string DisplayHolderName => "Inventory display";

        public InventoryDisplay(UIAssets assets) : base(assets.InventoryMenu, assets.Settings, assets.ButtonClickAudio)
        {
            _gameControls = new();
            _inventorySlot = assets.InventorySLot;
        }

        public override void OnInitialize()
        {
            GameServices.Register(this);
        }

        public override void OnSubscribe()
        {

        }

        public override void OnUnsubscribe()
        {

        }

        public override void OnClear()
        {
            GameServices.Deregister(this);
        }

        public override void Enable()
        {
            base.Enable();

            DisplayDocument.visualTreeAsset = Display;

            _gameControls.Menu.Enable();
            _gameControls.Menu.Back.performed += (c) => BackButtonClickedEventHandler();
            _gameControls.Menu.Inventory.performed += (c) => BackButtonClickedEventHandler();

            DisplayDocument.rootVisualElement.Q<Button>("back-button").clicked += BackButtonClickedEventHandler;

            var inventoryContainer = DisplayDocument.rootVisualElement.Q<VisualElement>("inventory-container");

            var infuseButton = DisplayDocument.rootVisualElement.Q<Button>("infuse-button");
            infuseButton.SetEnabled(false);
            infuseButton.clicked += InfuseButtonClickedEventHandler;

            int counter = 0;

            foreach (var item in s_player.Access.Inventory.GetContent())
            {
                var inventorySlot = _inventorySlot.CloneTree();
                inventorySlot.name = $"inventory-slot-{counter++}";

                var itemIcon = inventorySlot.Q<VisualElement>("item-icon");
                itemIcon.style.backgroundImage = new StyleBackground(item.Icon);
                itemIcon.style.backgroundColor = new StyleColor(item.RarityColor);

                /*
                var useItemButton = inventorySlot.Q<Button>("use-button");
                useItemButton.clicked += () => item.Use();

                if (s_gameModeLoader.Access.GameState == GameState.Level ||
                    (s_gameModeLoader.Access.GameState != GameState.Level && item.UsableOutsideOfLevel))
                {
                    useItemButton.SetEnabled(true);
                }
                else
                {
                    useItemButton.SetEnabled(false);
                }
                */

                inventoryContainer.Add(inventorySlot);
            }
        }

        protected override void Disable()
        {
            base.Disable();

            _gameControls.Menu.Disable();
            _gameControls.Menu.Back.performed -= (c) => BackButtonClickedEventHandler();
            _gameControls.Menu.Inventory.performed -= (c) => BackButtonClickedEventHandler();

            DisplayDocument.rootVisualElement.Q<Button>("back-button").clicked -= BackButtonClickedEventHandler;
            DisplayDocument.rootVisualElement.Q<Button>("infuse-button").clicked -= InfuseButtonClickedEventHandler;

            DisplayDocument.visualTreeAsset = null;
        }

        #region event handlers

        private void BackButtonClickedEventHandler()
        {
            Disable();
            ButtonClickAudio.PlayRandomAudioClip(Vector2.zero);

            if (s_gameModeLoader.Access.GameState == GameState.Level)
            {
                if (GameServices.TryGetService<HUDDisplay>(out var display) == true)
                {
                    display.Enable();
                }
                else
                {
                    throw new UnregisteredGameServiceAccessAttemptException(typeof(HUDDisplay));
                }
            }
            else
            {
                if (GameServices.TryGetService<MainMenuDisplay>(out var display) == true)
                {
                    display.Enable();
                }
                else
                {
                    throw new UnregisteredGameServiceAccessAttemptException(typeof(MainMenuDisplay));
                }
            }
        }

        private void InfuseButtonClickedEventHandler()
        {

        }

        #endregion
    }
}