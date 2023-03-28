using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Movement : MonoBehaviour, IEscapable, IMovementBehaviourSupplier
    {
        protected static readonly GameServiceFastAccess<MasterCameraHolder> MasterCameraHolder = new();
        protected static readonly GameServiceFastAccess<CameraShaker> CameraShaker = new();

        public event EventHandler Escaped;

        protected Rigidbody2D Body { get; private set; }
        protected MovementBehaviour MovementBehaviour { get; set; }
        protected MovementBehaviourSettings MovementSettings { get; set; }

        protected virtual void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            SetupRigidbody(Body);
        }

        protected virtual void OnDisable()
        {
            MovementBehaviour = null;
            Escaped = null;
        }

        protected virtual void FixedUpdate()
        {
            if (MovementBehaviour is null || MovementSettings is null)
            {
                return;
            }

            MovementBehaviour(Body, MovementSettings.Direction, MovementSettings.Speed);
        }

        protected virtual void SetupRigidbody(Rigidbody2D body)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.simulated = true;
            body.useFullKinematicContacts = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.sleepMode = RigidbodySleepMode2D.StartAwake;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void BeginWatchForEscape(Func<bool> escapeCondition)
        {
            StartCoroutine(AwaitEscape(escapeCondition));

            IEnumerator AwaitEscape(Func<bool> escapeCondition)
            {
                while (MasterCameraHolder.Access.InsideViewport(transform.position) == false)
                {
                    yield return null;
                }

                yield return null;

                while (MasterCameraHolder.Access.InsideViewport(transform.position) == true)
                {
                    yield return null;
                }

                while (escapeCondition() == false)
                {
                    yield return null;
                }

                yield return null;

                Escaped?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SupplyMovementBehaviour(MovementBehaviour behaviour, MovementBehaviourSettings settings)
        {
            MovementBehaviour = behaviour;
            MovementSettings = settings;
        }
    }
}
