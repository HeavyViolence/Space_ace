using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Forward propagation", menuName = "Space ace/Configs/Shooting/Projectles movement behaviour/Forward propagation")]
    public sealed class ForwardPropagation : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body, Vector2 direction, float speed)
        {
            Vector2 velocity = Time.fixedDeltaTime * speed * direction;
            body.MovePosition(body.position + velocity);
        };
}
}