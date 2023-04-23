using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Forward propagation", menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Forward propagation")]
    public sealed class ForwardPropagation : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body, MovementBehaviourSettings settings)
        {
            Vector2 velocity = Time.fixedDeltaTime * settings.Speed * settings.Direction;

            body.MovePosition(body.position + velocity);
        };
}
}