using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public delegate void MovementBehaviour(Rigidbody2D body, Vector2 direction, float speed);

    public interface IMovementBehaviourSupplier
    {
        void SupplyMovementBehaviour(MovementBehaviour behaviour, Vector2 direction, float speed);
    }
}