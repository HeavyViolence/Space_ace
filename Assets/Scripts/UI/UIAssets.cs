using SpaceAce.Main.Audio;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceAce.UI
{
    [CreateAssetMenu(fileName = "UI assets", menuName = "Space ace/Configs/UI assets")]
    public sealed class UIAssets : ScriptableObject
    {
        [SerializeField] private PanelSettings _settings;

        [SerializeField] private AudioCollection _buttonClickAudio;

        [SerializeField] private VisualTreeAsset _mainMenu;
        [SerializeField] private VisualTreeAsset _levelSelectionMenu;
        [SerializeField] private VisualTreeAsset _inventoryMenu;
        [SerializeField] private VisualTreeAsset _inventorySlot;
        [SerializeField] private VisualTreeAsset _HUDDisplay;
        [SerializeField] private VisualTreeAsset _pauseMenu;
        [SerializeField] private VisualTreeAsset _settingsMenu;
        [SerializeField] private VisualTreeAsset _creditsMenu;
        [SerializeField] private VisualTreeAsset _armoryMenu;
        [SerializeField] private VisualTreeAsset _overdriveMenu;

        public PanelSettings Settings => _settings;

        public AudioCollection ButtonClickAudio => _buttonClickAudio;

        public VisualTreeAsset MainMenu => _mainMenu;
        public VisualTreeAsset LevelSelectionMenu => _levelSelectionMenu;
        public VisualTreeAsset InventoryMenu => _inventoryMenu;
        public VisualTreeAsset InventorySLot => _inventorySlot;
        public VisualTreeAsset HUDDisplay => _HUDDisplay;
        public VisualTreeAsset PauseMenu => _pauseMenu;
        public VisualTreeAsset SettingsMenu => _settingsMenu;
        public VisualTreeAsset CreditsMenu => _creditsMenu;
        public VisualTreeAsset ArmoryMenu => _armoryMenu;
        public VisualTreeAsset OverdriveMenu => _overdriveMenu;
    }
}