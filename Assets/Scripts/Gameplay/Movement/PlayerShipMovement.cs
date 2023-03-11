using SpaceAce.Gameplay.Players;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class PlayerShipMovement : CollidableMovement, IPlayerShipMovementController
    {
        private const float SpawnPositionDisplacementFactor = 0.5f;

        protected override void OnEnable()
        {
            base.OnEnable();

            transform.position = new(0f, Config.LowerBound * SpawnPositionDisplacementFactor, 0f);
        }

        private Vector2 ClampMovementDirection(Vector2 rawMovementDirection)
        {
            float x = rawMovementDirection.x;
            float y = rawMovementDirection.y;

            if (Body.position.x < Config.LeftBound)
            {
                x = Mathf.Clamp(x, 0f, 1f);
            }

            if (Body.position.x > Config.RightBound)
            {
                x = Mathf.Clamp(x, -1f, 0f);
            }

            if (Body.position.y < Config.LowerBound)
            {
                y = Mathf.Clamp(y, 0f, 1f);
            }

            if (Body.position.y > Config.UpperBound)
            {
                y = Mathf.Clamp(y, -1f, 0f);
            }

            return new Vector2(x, y);
        }

        public void Move(Vector2 direction)
        {
            Vector2 clampedDirection = ClampMovementDirection(direction);
            Vector2 speed2D = new(Config.HorizontalSpeed, Config.VerticalSpeed);
            Vector2 velocity = Time.fixedDeltaTime * clampedDirection * speed2D;

            Body.MovePosition(Body.position + velocity);
        }
    }
}