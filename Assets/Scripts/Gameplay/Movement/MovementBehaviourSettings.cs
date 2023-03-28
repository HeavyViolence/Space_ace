using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class MovementBehaviourSettings
    {
        public Vector2 Direction { get; }
        public float Speed { get; }

        public MovementBehaviourSettings(Vector2 direction, float speed)
        {
            Direction = direction;
            Speed = speed;
        }
    }
}