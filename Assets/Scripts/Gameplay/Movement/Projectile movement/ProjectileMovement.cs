using SpaceAce.Gameplay.Shooting;
using System;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [RequireComponent(typeof(DamageDealer))]
    public abstract class ProjectileMovement : Movement, IMovementBehaviourSupplier
    {
        protected override void SetupRigidbody(Rigidbody2D body)
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            body.simulated = true;
            body.useAutoMass = false;
            body.mass = 1f;
            body.drag = 0f;
            body.angularDrag = 0f;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.sleepMode = RigidbodySleepMode2D.StartAwake;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void SupplyMovementBehaviour(Action<Rigidbody2D> behaviour) => MovementBehaviour = behaviour;
    }
}