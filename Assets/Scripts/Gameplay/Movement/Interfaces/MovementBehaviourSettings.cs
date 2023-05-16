using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class MovementBehaviourSettings
    {
        public Vector3 InitialDirection { get; private set; }
        public float TopSpeed { get; private set; }
        public float TopSpeedGainDuration { get; private set; }
        public float RevolutionsPerMinute { get; private set; }
        public Transform Target { get; private set; }
        public float TargetSeekingSpeed { get; private set; }

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