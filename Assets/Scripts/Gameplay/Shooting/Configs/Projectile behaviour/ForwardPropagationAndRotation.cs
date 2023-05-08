using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Forward propagation and rotation", menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Forward propagation and rotation")]
    public sealed class ForwardPropagationAndRotation : ProjectileBehaviour
    {
        private const float DegreesPerRevolution = 360f;

        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body, MovementBehaviourSettings settings, ref float timer)
        {
            timer += Time.fixedDeltaTime;

            float speedFactor = Mathf.Clamp01(timer / settings.TopSpeedGainDuration);
            float speed = settings.TopSpeed * speedFactor;
            Vector2 velocity = Time.fixedDeltaTime * speed * settings.Direction;

            float angularSpeed = Time.fixedDeltaTime * settings.RevolutionsPerMinute * DegreesPerRevolution;

            body.MovePosition(body.position + velocity);
            body.MoveRotation(body.rotation + angularSpeed);
        };
    }
}