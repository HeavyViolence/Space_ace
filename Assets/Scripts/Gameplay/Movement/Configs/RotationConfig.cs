using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public enum RotationDirection
    {
        Left, Right, Random
    }

    [CreateAssetMenu(fileName = "Rotation config", menuName = "Space ace/Configs/Movement/Rotation config")]
    public sealed class RotationConfig : ScriptableObject
    {
        public const float MinRPM = 0f;
        public const float MaxRPM = 60f;
        public const float DefaultRPM = 15f;

        public const float MinTurningRadius = 10f;
        public const float MaxTurningRadius = 100f;

        [SerializeField] private RotationDirection _rotationDirection = RotationDirection.Left;

        [SerializeField] private float _rpm = DefaultRPM;
        [SerializeField] private float _rpmRandomDeviation = 0f;

        [SerializeField] private float _turningRadius = MinTurningRadius;
        [SerializeField] private float _turningRadiusRandomDeviation = 0f;

        public RangedFloat RevolutionsPerMinute { get; private set; }
        public RangedFloat TurningRadius { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            switch (_rotationDirection)
            {
                case RotationDirection.Left:
                    {
                        RevolutionsPerMinute = new(-1f * _rpm, _rpmRandomDeviation);

                        break;
                    }
                case RotationDirection.Right:
                    {
                        RevolutionsPerMinute = new(_rpm, _rpmRandomDeviation);

                        break;
                    }
                case RotationDirection.Random:
                    {
                        RevolutionsPerMinute = new(_rpm * AuxMath.RandomNormal, _rpmRandomDeviation);

                        break;
                    }
            }

            TurningRadius = new(_turningRadius, _turningRadiusRandomDeviation);
        }
    }
}