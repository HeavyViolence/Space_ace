using SpaceAce.Auxiliary;
using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [CreateAssetMenu(fileName = "Forward propagation and rotation",
                     menuName = "Space ace/Configs/Shooting/Projectiles movement behaviour/Forward propagation and rotation")]
    public sealed class ForwardPropagationAndRotation : ProjectileBehaviour
    {
        public override MovementBehaviour Behaviour => delegate (Rigidbody2D body,
                                                                 MovementBehaviourSettings settings,
                                                                 ref MovementAuxiliaryData data)
        {
            data.Timer += Time.fixedDeltaTime;

            float speedFactor = Mathf.Clamp01(data.Timer / settings.TopSpeedGainDuration);
            float speed = settings.TopSpeed * speedFactor;
            Vector2 velocity = Time.fixedDeltaTime * speed * settings.InitialDirection;

            float angularSpeed = Time.fixedDeltaTime * settings.RevolutionsPerMinute * AuxMath.DegreesPerRevolution / AuxMath.SecondsPerMinute;

            body.MovePosition(body.position + velocity);
            body.MoveRotation(body.rotation + angularSpeed);
        };
    }
}