using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class MovementBehaviourSettings
    {
        public Vector2 Direction { get; private set; }
        public float TopSpeed { get; private set; }
        public float TopSpeedGainDuration { get; private set; }
        public float RevolutionsPerMinute { get; private set; }
        public Transform Target { get; private set; }
        public float TargetSeekingSpeed { get; private set; }

        public MovementBehaviourSettings(Vector2 direction,
                                         float topSpeed,
                                         float topSpeedGainDuration,
                                         float revolutionsPerMinute,
                                         Transform target,
                                         float targetSeekingSpeed)
        {
            Direction = direction;
            TopSpeed = topSpeed;
            TopSpeedGainDuration = topSpeedGainDuration;
            RevolutionsPerMinute = revolutionsPerMinute;
            Target = target;
            TargetSeekingSpeed = targetSeekingSpeed;
        }
    }
}