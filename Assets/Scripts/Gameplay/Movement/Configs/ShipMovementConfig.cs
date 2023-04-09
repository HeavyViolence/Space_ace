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

        public RangedFloat HorizontalSpeedDuration { get; private set; }
        public RangedFloat HorizontalSpeedTransitionDuration { get; private set; }

        public RangedFloat VerticalSpeedDuration { get; private set; }
        public RangedFloat VerticalSpeedTransitionDuration { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public sealed override void ApplySettings()
        {
            base.ApplySettings();

            HorizontalSpeedDuration = new(_horizontalSpeedDuration, _horizontalSpeedDurationRandomDeviation);
            HorizontalSpeedTransitionDuration = new(_horizontalSpeedTransitionDuration, _horizontalSpeedTransitionDurationRandomDeviation);

            VerticalSpeedDuration = new(_verticalSpeedDuration, _verticalSpeedDurationRandomDeviation);
            VerticalSpeedTransitionDuration = new(_verticalSpeedTranstitionDuration, _verticalSpeedTranstitionDurationRandomDeviation);
        }
    }
}