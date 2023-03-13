using SpaceAce.Architecture;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Movement : MonoBehaviour, IEscapable
    {
        protected static readonly GameServiceFastAccess<MasterCameraHolder> MasterCameraHolder = new();
        protected static readonly GameServiceFastAccess<CameraShaker> CameraShaker = new();

        public event EventHandler Escaped;

        protected Rigidbody2D Body { get; private set; } = null;
        protected Action<Rigidbody2D> MovementBehaviour { get; set; } = null;
        protected bool ApplyMovementBehaviourOnEveryPhysicsUpdate { get; set; } = false;

        protected virtual void Awake()
        {
            Body = GetComponent<Rigidbody2D>();

            Body.bodyType = RigidbodyType2D.Kinematic;
            Body.simulated = true;
            Body.useFullKinematicContacts = true;
            Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Body.sleepMode = RigidbodySleepMode2D.StartAwake;
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        protected virtual void OnDisable()
        {
            ApplyMovementBehaviourOnEveryPhysicsUpdate = false;
            MovementBehaviour = null;
            Escaped = null;
        }

        protected virtual void FixedUpdate()
        {
            if (MovementBehaviour is null)
            {
                return;
            }

            MovementBehaviour(Body);
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
    }
}
