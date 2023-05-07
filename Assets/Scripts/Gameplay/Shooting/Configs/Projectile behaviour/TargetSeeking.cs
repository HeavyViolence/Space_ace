using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Target seeking", menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Target seeking")]
    public sealed class TargetSeeking : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body, MovementBehaviourSettings settings)
        {
            if (settings.Target != null)
            {
                Vector2 targetDirection = (settings.Target.position - body.transform.position).normalized;
                Vector2 currentDirection = body.transform.up;
                Vector2 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, settings.TurningSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1f).normalized;
                Quaternion newRotation = Quaternion.LookRotation(newDirection);
                Vector2 velocity = Time.fixedDeltaTime * settings.Speed * newDirection;

                body.MovePosition(body.position + velocity);
                body.MoveRotation(newRotation.eulerAngles.z);
            }
            else
            {
                Vector2 velocity = Time.fixedDeltaTime * settings.Speed * settings.Direction;

                body.MovePosition(body.position + velocity);
            }
        };
    }
}