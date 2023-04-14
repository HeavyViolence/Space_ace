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
        [SerializeField] private VisualTreeAsset _playMenu;

        public PanelSettings Settings => _settings;

        public AudioCollection ButtonClickAudio => _buttonClickAudio;

        public VisualTreeAsset MainMenu => _mainMenu;
        public VisualTreeAsset PlayMenu => _playMenu;
    }
}