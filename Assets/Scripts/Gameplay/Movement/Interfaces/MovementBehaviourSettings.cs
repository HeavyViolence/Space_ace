using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class MovementBehaviourSettings
    {
        public Vector2 Direction { get; private set; }
        public float Speed { get; private set; }
        public float RevolutionsPerMinute { get; private set; }
        public float TurningSpeed { get; private set; }
        public Transform Target { get; private set; }

        public MovementBehaviourSettings(Vector2 direction,
                                         float speed,
                                         float revolutionsPerMinute,
                                         float turningSpeed,
                                         Transform target)
        {
            Direction = direction;
            Speed = speed;
            RevolutionsPerMinute = revolutionsPerMinute;
            TurningSpeed = turningSpeed;
            Target = target;
        }
    }
}