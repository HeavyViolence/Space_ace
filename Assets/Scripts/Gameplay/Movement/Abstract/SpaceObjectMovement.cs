using SpaceAce.Auxiliary;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public abstract class SpaceObjectMovement : CollidableMovement
    {
        [SerializeField] private RotationConfig _rotationConfig;

        private MovementBehaviour SpaceObjectFlyby => delegate (Rigidbody2D body,
                                                                MovementBehaviourSettings settings,
                                                                ref MovementAuxiliaryData data)
        {
            Vector2 velocity = Time.fixedDeltaTime * settings.TopSpeed * settings.InitialDirection;
            float angularSpeed = Time.fixedDeltaTime * settings.RevolutionsPerMinute * AuxMath.DegreesPerRevolution / AuxMath.SecondsPerMinute;

            body.MovePosition(body.position + velocity);
            body.MoveRotation(body.rotation + angularSpeed);
        };

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(MovementSetup());
        }

        private IEnumerator MovementSetup()
        {
            yield return null;

            MovementBehaviourSettings = new(GetMovementDirection(),
                                            Config.VerticalSpeed.RandomValue,
                                            0f,
                                            _rotationConfig.RevolutionsPerMinute.RandomValue,
                                            null,
                                            0f);

            MovementBehaviour = SpaceObjectFlyby;
        }

        protected virtual Vector3 GetMovementDirection()
        {
            float targetPositionX = Random.Range(MasterCameraHolder.Access.ViewportLeftBound,
                                                 MasterCameraHolder.Access.ViewportRightBound);
            float targetPositionY = MasterCameraHolder.Access.ViewportLowerBound;

            Vector3 targetPosition = new(targetPositionX, targetPositionY, 0f);

            return (targetPosition - transform.position).normalized;
        }
    }
}