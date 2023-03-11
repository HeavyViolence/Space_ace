using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public interface IMovementBehaviourSupplier
    {
        void SupplyMovementBehaviour(Action<Rigidbody2D> behaviour, bool applyOnEveryPhysicsUpdate);
    }
}