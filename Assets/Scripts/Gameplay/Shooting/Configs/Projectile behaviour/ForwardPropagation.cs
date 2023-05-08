using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Forward propagation", menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Forward propagation")]
    public sealed class ForwardPropagation : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body, MovementBehaviourSettings settings, ref float timer)
        {
            timer += Time.fixedDeltaTime;

            float speedFactor = Mathf.Clamp01(timer / settings.TopSpeedGainDuration);
            float speed = settings.TopSpeed * speedFactor;
            Vector2 velocity = Time.fixedDeltaTime * speed * settings.Direction;

            body.MovePosition(body.position + velocity);
        };
}
}