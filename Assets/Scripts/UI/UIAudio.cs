using SpaceAce.Main.Audio;
using UnityEngine;

namespace SpaceAce.UI
{
    [CreateAssetMenu(fileName = "UI audio", menuName = "Space ace/Configs/UI/UI audio")]
    public sealed class UIAudio : ScriptableObject
    {
        [SerializeField] private AudioCollection _forwardButtonClick;
        [SerializeField] private AudioCollection _backButtonClick;

        [SerializeField] private AudioCollection _itemBought;
        [SerializeField] private AudioCollection _itemSold;

        [SerializeField] private AudioCollection _error;

        [SerializeField] private AudioCollection _lost;
        [SerializeField] private AudioCollection _win;

        [SerializeField] private AudioCollection _powerup;

        public AudioCollection ForwardButtonClick => _forwardButtonClick;
        public AudioCollection BackButtonClick => _backButtonClick;

        public AudioCollection ItemBought => _itemBought;
        public AudioCollection ItemSold => _itemSold;

        public AudioCollection Error => _error;

        public AudioCollection Lost => _lost;
        public AudioCollection Win => _win;

        public AudioCollection Powerup => _powerup;
    }
}