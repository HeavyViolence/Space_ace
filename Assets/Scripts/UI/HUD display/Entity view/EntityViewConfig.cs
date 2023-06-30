using UnityEngine;

namespace SpaceAce.UI
{
    [CreateAssetMenu(fileName = "Entity view config", menuName = "Space ace/Configs/Entity view config")]
    public sealed class EntityViewConfig : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private bool _playerView;

        public Sprite Icon => _icon;
        public bool PlayerView => _playerView;
    }
}