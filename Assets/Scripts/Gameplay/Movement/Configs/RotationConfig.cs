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

        [SerializeField] private RotationDirection _rotationDirection = RotationDirection.Left;

        [SerializeField] private float _rpm = DefaultRPM;
        [SerializeField] private float _rpmRandomDeviation = 0f;

        public RangedFloat RotationsPerMinute { get; private set; }

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
                        RotationsPerMinute = new(-1f * _rpm, _rpmRandomDeviation);

                        break;
                    }
                case RotationDirection.Right:
                    {
                        RotationsPerMinute = new(_rpm, _rpmRandomDeviation);

                        break;
                    }
                case RotationDirection.Random:
                    {
                        RotationsPerMinute = new(_rpm * AuxMath.RandomNormal, _rpmRandomDeviation);

                        break;
                    }
            }
        }
    }
}