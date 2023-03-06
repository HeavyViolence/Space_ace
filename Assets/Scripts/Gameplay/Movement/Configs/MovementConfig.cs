using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main;
using SpaceAce.Main.Audio;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [CreateAssetMenu(fileName = "Movement config", menuName = "Space ace/Configs/Movement config")]
    public class MovementConfig : ScriptableObject
    {
        public const float MaxSpeed = 50f;

        public const float MaxBoundDisplacement = 2f;
        public const float DefaultBoundDisplacement = 1f;

        public const float MinCollisionDamage = 100f;
        public const float MaxCollisionDamage = 10000f;
        public const float DefaultCollisionDamage = 1000f;

        [SerializeField] private float _horizontalSpeed = 0f;
        [SerializeField] private float _horizontalSpeedRandomDeviation = 0f;

        [SerializeField] private float _verticalSpeed = 0f;
        [SerializeField] private float _verticalSpeedRandomDeviation = 0f;

        [SerializeField] private bool _customBoundsEnabled = false;
        [SerializeField] private float _upperBoundDisplacement = DefaultBoundDisplacement;
        [SerializeField] private float _lowerBoundDisplacement = DefaultBoundDisplacement;
        [SerializeField] private float _sideBoundsDisplacement = DefaultBoundDisplacement;

        [SerializeField] private bool _collisionDamageEnabled = false;
        [SerializeField] private float _collisionDamage = DefaultCollisionDamage;
        [SerializeField] private float _collisionDamageRandomDeviation = 0f;
        [SerializeField] private AudioCollection _collisionAudio = null;
        [SerializeField] private bool _cameraShakeOnCollisionEnabled = false;

        private readonly GameServiceFastAccess<MasterCameraHolder> _masterCameraHolder = new();

        public float HorizontalSpeed => _horizontalSpeed + _horizontalSpeedRandomDeviation * AuxMath.RandomNormal;
        public float VerticalSpeed => _verticalSpeed + _verticalSpeedRandomDeviation * AuxMath.RandomNormal;
        public float Speed2D
        {
            get
            {
                if (HorizontalSpeed == 0f && VerticalSpeed == 0f)
                {
                    return 0f;
                }

                if (HorizontalSpeed != 0f && VerticalSpeed == 0f)
                {
                    return HorizontalSpeed;
                }

                if (HorizontalSpeed == 0f && VerticalSpeed != 0f)
                {
                    return VerticalSpeed;
                }

                return Mathf.Sqrt(HorizontalSpeed * HorizontalSpeed + VerticalSpeed + VerticalSpeed);
            }
        }

        public bool CustomBoundsEnabled => _customBoundsEnabled;
        public float LeftBound => CustomBoundsEnabled ? _masterCameraHolder.Access.ViewportLeftBound * _sideBoundsDisplacement
                                                      : _masterCameraHolder.Access.ViewportLeftBound;
        public float RightBound => CustomBoundsEnabled ? _masterCameraHolder.Access.ViewportRightBound * _sideBoundsDisplacement
                                                       : _masterCameraHolder.Access.ViewportRightBound;
        public float UpperBound => CustomBoundsEnabled ? _masterCameraHolder.Access.ViewportUpperBound * _upperBoundDisplacement
                                                       : _masterCameraHolder.Access.ViewportUpperBound;
        public float LowerBound => CustomBoundsEnabled ? _masterCameraHolder.Access.ViewportLowerBound * _lowerBoundDisplacement
                                                       : _masterCameraHolder.Access.ViewportLowerBound;

        public bool CollisionDamageEnabled => _collisionDamageEnabled;
        public float CollisionDamage => _collisionDamage * _collisionDamageRandomDeviation * AuxMath.RandomNormal;
        public AudioCollection CollisionAudio => _collisionAudio;
        public bool CameraShakeOnCollisionEnabled => _cameraShakeOnCollisionEnabled;
    }
}