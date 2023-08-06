using SpaceAce.Architecture;
using SpaceAce.Auxiliary.StateMachines;
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
    public abstract class EnemyMovement : MonoBehaviourStateMachine, IEscapable, IExperienceSource
    {
        private static readonly GameServiceFastAccess<CameraShaker> s_cameraShaker = new();
        protected static readonly GameServiceFastAccess<GamePauser> GamePauser = new();
        protected static readonly GameServiceFastAccess<MasterCameraHolder> MasterCameraHolder = new();

        public event EventHandler Escaped;

        [SerializeField] private ShipMovementConfig _config;

        private Coroutine _escapeAwaitingRoutine;

        private DamageDealer _collisionDamageDealer;

        protected ShipMovementConfig Config => _config;

        public virtual float NextHorizontalSpeed => _config.HorizontalSpeed.RandomValue;
        public virtual float NextHorizontalSpeedDuration => _config.HorizontalSpeedDuration.RandomValue;
        public virtual float NextHorizontalSpeedTransitionDuration => _config.HorizontalSpeedTransitionDuration.RandomValue;
        public virtual float NextVerticalSpeed => _config.VerticalSpeed.RandomValue;
        public virtual float NextVerticalSpeedDuration => _config.VerticalSpeedDuration.RandomValue;
        public virtual float NextVerticalSpeedTransitionDuration => _config.VerticalSpeedTransitionDuration.RandomValue;
        public virtual float NextCollisionDamage => _config.CollisionDamageEnabled ? _config.CollisionDamage.RandomValue : 0f;

        public virtual float LeftBound => _config.LeftBound;
        public virtual float RightBound => _config.RightBound;
        public virtual float UpperBound => _config.UpperBound;
        public virtual float LowerBound => _config.LowerBound;

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
        }

        protected override void OnDeinitialize()
        {
            _collisionDamageDealer.Hit -= CollisionHitEventHandler;
            StopWatchingForEscape();
        }

        protected override void OnUpdate()
        {
            if (GamePauser.Access.Paused == true) return;

            base.OnUpdate();
        }

        protected override void OnFixedUpdate()
        {
            if (GamePauser.Access.Paused == true) return;

            base.OnFixedUpdate();
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
                e.DamageReceiver?.ApplyDamage(NextCollisionDamage);
                _config.CollisionAudio.PlayRandomAudioClip(e.HitPosition);

                if (_config.CameraShakeOnCollisionEnabled) s_cameraShaker.Access.ShakeOnCollision();
            }
        }

        public void StartWatchingForEscape(Func<bool> escapeCondition)
        {
            _escapeAwaitingRoutine = StartCoroutine(AwaitEscape(escapeCondition));

            IEnumerator AwaitEscape(Func<bool> escapeCondition)
            {
                while (MasterCameraHolder.Access.InsideViewport(transform.position) == false) yield return null;
                yield return null;

                while (MasterCameraHolder.Access.InsideViewport(transform.position) == true) yield return null;

                yield return null;

                while (escapeCondition() == false) yield return null;

                Escaped?.Invoke(this, EventArgs.Empty);
                _escapeAwaitingRoutine = null;
            }
        }

        public void StopWatchingForEscape()
        {
            if (_escapeAwaitingRoutine != null)
            {
                StopCoroutine(_escapeAwaitingRoutine);
                _escapeAwaitingRoutine = null;
            }

            Escaped = null;
        }

        public float GetExperience()
        {
            float value = 0f;

            value += Config.HorizontalSpeed.MaxValue / Config.HorizontalSpeedTransitionDuration.MinValue;
            value += Config.VerticalSpeed.MaxValue / Config.VerticalSpeedTransitionDuration.MinValue;

            return value;
        }
    }
}