using SpaceAce.Architecture;
using SpaceAce.Auxiliary.StateMachines;
using SpaceAce.Gameplay.Amplifications;
using SpaceAce.Gameplay.Experience;
using SpaceAce.Gameplay.Shooting;
using SpaceAce.Main;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement.EnemyMovement
{
    [RequireComponent(typeof(DamageDealer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyMovement : MonoBehaviourStateMachine, IEscapable, IExperienceSource, IAmplifiable
    {
        private static readonly GameServiceFastAccess<CameraShaker> s_cameraShaker = new();
        protected static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();

        public event EventHandler Escaped;

        [SerializeField] private ShipMovementConfig _config;

        private DamageDealer _collisionDamageDealer;

        private float _speedAmplifier = 1f;
        private float _speedDurationAmplifier = 1f;
        private float _speedTransitionDurationAmplifier = 1f;
        private float _collisionDmageAmplifier = 1f;

        public float HorizontalSpeed => _config.HorizontalSpeed.RandomValue * _speedAmplifier;
        public float HorizontalSpeedDuration => _config.HorizontalSpeedDuration.RandomValue * _speedDurationAmplifier;
        public float HorizontalSpeedTransitionDuration => _config.HorizontalSpeedTransitionDuration.RandomValue * _speedTransitionDurationAmplifier;
        public float VerticalSpeed => _config.VerticalSpeed.RandomValue * _speedAmplifier;
        public float VerticalSpeedDuration => _config.VerticalSpeedDuration.RandomValue * _speedDurationAmplifier;
        public float VerticalSpeedTransitionDuration => _config.VerticalSpeedTransitionDuration.RandomValue * _speedTransitionDurationAmplifier;
        public float LeftBound => _config.LeftBound;
        public float RightBound => _config.RightBound;
        public float UpperBound => _config.UpperBound;
        public float LowerBound => _config.LowerBound;
        public float CollisionDamage => _config.CollisionDamageEnabled ? _config.CollisionDamage.RandomValue * _collisionDmageAmplifier : 0f;


        public Vector2 PreviousStateExitVelocity { get; set; }
        public Rigidbody2D Body { get; private set; }

        protected override void OnSetup()
        {
            Body = SetupRigidbody2D();
            _collisionDamageDealer = gameObject.GetComponent<DamageDealer>();
        }

        protected override void OnInitialize()
        {
            _collisionDamageDealer.Hit += CollisionHitEventHandler;

            _speedAmplifier = 1f;
            _speedDurationAmplifier = 1f;
            _speedTransitionDurationAmplifier = 1f;
            _collisionDmageAmplifier = 1f;
        }

        private void OnDisable()
        {
            _collisionDamageDealer.Hit -= CollisionHitEventHandler;
            Escaped = null;
        }

        private Rigidbody2D SetupRigidbody2D()
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();

            body.bodyType = RigidbodyType2D.Kinematic;
            body.simulated = true;
            body.useFullKinematicContacts = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            body.sleepMode = RigidbodySleepMode2D.StartAwake;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            return body;
        }

        private void CollisionHitEventHandler(object sender, HitEventArgs e)
        {
            if (_config.CollisionDamageEnabled)
            {
                e.DamageReceiver?.ApplyDamage(CollisionDamage);
                _config.CollisionAudio.PlayRandomAudioClip(e.HitPosition);

                if (_config.CameraShakeOnCollisionEnabled)
                {
                    s_cameraShaker.Access.ShakeOnCollision();
                }
            }
        }

        public void BeginWatchForEscape(Func<bool> escapeCondition)
        {
            StartCoroutine(AwaitEscape(escapeCondition));

            IEnumerator AwaitEscape(Func<bool> escapeCondition)
            {
                while (s_masterCameraHolder.Access.InsideViewport(transform.position) == false)
                {
                    yield return null;
                }

                yield return null;

                while (s_masterCameraHolder.Access.InsideViewport(transform.position) == true)
                {
                    yield return null;
                }

                yield return null;

                while (escapeCondition() == false)
                {
                    yield return null;
                }

                Escaped?.Invoke(this, EventArgs.Empty);
            }
        }

        public float GetExperience()
        {
            float value = 0f;

            value += _config.HorizontalSpeed.MaxValue / _config.HorizontalSpeedTransitionDuration.MinValue;
            value += _config.VerticalSpeed.MaxValue / _config.VerticalSpeedTransitionDuration.MinValue;

            return value;
        }

        public void Amplify(float factor)
        {
            _speedAmplifier *= factor;
            _speedDurationAmplifier /= factor;
            _speedTransitionDurationAmplifier /= factor;
            _collisionDmageAmplifier *= factor;
        }
    }
}