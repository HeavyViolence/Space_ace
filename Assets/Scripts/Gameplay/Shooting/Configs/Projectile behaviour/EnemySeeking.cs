using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Enemy seeking", menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Enemy seeking")]
    public sealed class EnemySeeking : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body, MovementBehaviourSettings settings, ref float timer)
        {
            timer += Time.fixedDeltaTime;

            float speedFactor = Mathf.Clamp01(timer / settings.TopSpeedGainDuration);
            float speed = settings.TopSpeed * speedFactor;

            if (settings.Target == null)
            {
                Vector2 velocity = Time.fixedDeltaTime * speed * settings.Direction;

                body.MovePosition(body.position + velocity);
            }
            else
            {
                Vector2 currentDirection = body.transform.up;
                Vector2 targetDirection = (settings.Target.position - body.transform.position).normalized;
                Vector2 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, settings.TargetSeekingSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1f).normalized;
                Vector2 velocity = Time.fixedDeltaTime * speed * newDirection;

                Quaternion newRotation = Quaternion.LookRotation(newDirection);

                body.MovePosition(body.position + velocity);
                body.MoveRotation(newRotation.eulerAngles.z);
            }
        };
    }
}