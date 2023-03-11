using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main;
using SpaceAce.Main.Audio;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [CreateAssetMenu(fileName = "Movement config", menuName = "Space ace/Configs/Movement/Movement config")]
    public class MovementConfig : ScriptableObject
    {
        public const float MaxSpeed = 100f;

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

        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();

        public float MinHorizontalSpeed => _horizontalSpeed - _horizontalSpeedRandomDeviation;
        public float MaxHorizontalSpeed => _horizontalSpeed + _horizontalSpeedRandomDeviation;
        public float HorizontalSpeed => _horizontalSpeed + _horizontalSpeedRandomDeviation * AuxMath.RandomNormal;

        public float MinVerticalSpeed => _verticalSpeed - _verticalSpeedRandomDeviation;
        public float MaxVerticalSpeed => _verticalSpeed + _verticalSpeedRandomDeviation;
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

                return Mathf.Sqrt(HorizontalSpeed * HorizontalSpeed + VerticalSpeed * VerticalSpeed);
            }
        }

        public bool CustomBoundsEnabled => _customBoundsEnabled;
        public float LeftBound => CustomBoundsEnabled ? s_masterCameraHolder.Access.ViewportLeftBound * _sideBoundsDisplacement
                                                      : s_masterCameraHolder.Access.ViewportLeftBound;
        public float RightBound => CustomBoundsEnabled ? s_masterCameraHolder.Access.ViewportRightBound * _sideBoundsDisplacement
                                                       : s_masterCameraHolder.Access.ViewportRightBound;
        public float UpperBound => CustomBoundsEnabled ? s_masterCameraHolder.Access.ViewportUpperBound * _upperBoundDisplacement
                                                       : s_masterCameraHolder.Access.ViewportUpperBound;
        public float LowerBound => CustomBoundsEnabled ? s_masterCameraHolder.Access.ViewportLowerBound * _lowerBoundDisplacement
                                                       : s_masterCameraHolder.Access.ViewportLowerBound;

        public bool CollisionDamageEnabled => _collisionDamageEnabled;
        public float CollisionDamage => CollisionDamageEnabled ? _collisionDamage + _collisionDamageRandomDeviation * AuxMath.RandomNormal : 0f;
        public AudioCollection CollisionAudio => _collisionAudio;
        public bool CameraShakeOnCollisionEnabled => _cameraShakeOnCollisionEnabled;
    }
}