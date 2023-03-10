using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [CreateAssetMenu(fileName = "Ship movement config", menuName = "Space ace/Configs/Movement/Ship movement config")]
    public sealed class ShipMovementConfig : MovementConfig
    {
        public const float MinSpeedDuration = 0.5f;
        public const float MaxSpeedDuration = 5f;
        public const float DefaultSpeedDuration = 2f;

        public const float MinSpeedTransitionDuration = 0.25f;
        public const float MaxSpeedTransitionDuration = 5f;
        public const float DefaultSpeedTransitionDuration = 2f;

        [SerializeField] private float _horizontalSpeedDuration = DefaultSpeedDuration;
        [SerializeField] private float _horizontalSpeedDurationRandomDeviation = 0f;

        [SerializeField] private float _horizontalSpeedTransitionDuration = DefaultSpeedTransitionDuration;
        [SerializeField] private float _horizontalSpeedTransitionDurationRandomDeviation = 0f;

        [SerializeField] private float _verticalSpeedDuration = DefaultSpeedDuration;
        [SerializeField] private float _verticalSpeedDurationRandomDeviation = 0f;

        [SerializeField] private float _verticalSpeedTranstitionDuration = DefaultSpeedTransitionDuration;
        [SerializeField] private float _verticalSpeedTranstitionDurationRandomDeviation = 0f;

        public float HorizontalSpeedDuration => _horizontalSpeedDuration +
                                                _horizontalSpeedDurationRandomDeviation * AuxMath.RandomNormal;
        public float HorizontalSpeedTransitionDuration => _horizontalSpeedTransitionDuration +
                                                          _horizontalSpeedTransitionDurationRandomDeviation * AuxMath.RandomNormal;
        public float VerticalSpeedDuration => _verticalSpeedDuration +
                                              _verticalSpeedDurationRandomDeviation * AuxMath.RandomNormal;
        public float VerticalSpeedTransitionDuration => _verticalSpeedTranstitionDuration +
                                                        _verticalSpeedTranstitionDurationRandomDeviation * AuxMath.RandomNormal;
    }
}