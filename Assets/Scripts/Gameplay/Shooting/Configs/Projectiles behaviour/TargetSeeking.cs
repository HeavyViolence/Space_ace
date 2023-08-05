using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Target seeking", menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Target seeking")]
    public sealed class TargetSeeking : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body,
                                                                 MovementBehaviourSettings settings,
                                                                 ref MovementAuxiliaryData data)
        {
            data.Timer += Time.fixedDeltaTime;

            float speedFactor = Mathf.Clamp01(data.Timer / settings.TopSpeedGainDuration);
            float speed = settings.TopSpeed * speedFactor;
            Vector2 velocity;

            if (data.CurrentDirection == Vector3.zero) data.CurrentDirection = settings.InitialDirection;

            if (settings.Target == null)
            {
                velocity = Time.fixedDeltaTime * speed * settings.InitialDirection;
                body.MovePosition(body.position + velocity);
            }
            else
            {
                if (data.CurrentRotation == Quaternion.identity)
                    data.CurrentRotation = Quaternion.LookRotation(Vector3.forward, settings.InitialDirection);

                if (settings.Target.gameObject.activeInHierarchy == true)
                {
                    Vector3 targetDirection = (settings.Target.position - body.transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

                    data.CurrentRotation = Quaternion.RotateTowards(data.CurrentRotation, targetRotation, settings.TargetSeekingSpeed * Time.fixedDeltaTime);
                    data.CurrentDirection = Vector3.RotateTowards(data.CurrentDirection,
                                                                  targetDirection,
                                                                  settings.TargetSeekingSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,
                                                                  1f).normalized;

                    velocity = Time.fixedDeltaTime * speed * data.CurrentDirection;

                    body.MovePosition(body.position + velocity);
                    body.MoveRotation(data.CurrentRotation);
                }
                else
                {
                    velocity = Time.fixedDeltaTime * speed * data.CurrentDirection;
                    body.MovePosition(body.position + velocity);
                }
            }
        };
    }
}