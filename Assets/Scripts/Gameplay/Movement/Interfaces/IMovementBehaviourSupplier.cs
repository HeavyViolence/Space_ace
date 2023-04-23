using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public delegate void MovementBehaviour(Rigidbody2D body, MovementBehaviourSettings settings);

    public interface IMovementBehaviourSupplier
    {
        void SupplyMovementBehaviour(MovementBehaviour behaviour, MovementBehaviourSettings settings);
    }
}