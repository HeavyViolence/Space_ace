using SpaceAce.Auxiliary;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public enum RotationDirection
    {
        Left, Right, Random
    }

    [CreateAssetMenu(fileName = "Rotation config", menuName = "Space ace/Configs/Rotation config")]
    public sealed class RotationConfig : ScriptableObject
    {
        private const float DegreesPerSecondPerRotationPerMinute = 5f;

        public const float MinRPM = 0f;
        public const float MaxRPM = 60f;
        public const float DefaultRPM = 15f;

        [SerializeField] private RotationDirection _rotationDirection = RotationDirection.Left;

        [SerializeField] private float _rpm = DefaultRPM;
        [SerializeField] private float _rpmRandomDeviation = 0f;

        private float RandomizedRPM => _rpm + _rpmRandomDeviation * AuxMath.RandomNormal;

        public float SignedRotationsPerMinute
        {
            get
            {
                switch (_rotationDirection)
                {
                    case RotationDirection.Left:
                        {
                            return -1f * RandomizedRPM;
                        }
                    case RotationDirection.Right:
                        {
                            return RandomizedRPM;
                        }
                    case RotationDirection.Random:
                        {
                            return RandomizedRPM * AuxMath.RandomSign;
                        }
                }

                return Mathf.NegativeInfinity;
            }
        }

        public float SignedDegreesPerSecond => SignedRotationsPerMinute * DegreesPerSecondPerRotationPerMinute;
    }
}