using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class MovementBehaviourSettings
    {
        public Vector3 InitialDirection { get; }
        public float TopSpeed { get; }
        public float TopSpeedGainDuration { get; }
        public float RevolutionsPerMinute { get; }
        public Transform Target { get; }
        public float TargetSeekingSpeed { get; }

        public MovementBehaviourSettings(Vector3 initialDirection,
                                         float topSpeed,
                                         float topSpeedGainDuration,
                                         float revolutionsPerMinute,
                                         Transform target,
                                         float targetSeekingSpeed)
        {
            InitialDirection = initialDirection;
            TopSpeed = topSpeed;
            TopSpeedGainDuration = topSpeedGainDuration;
            RevolutionsPerMinute = revolutionsPerMinute;
            Target = target;
            TargetSeekingSpeed = targetSeekingSpeed;
        }
    }
}