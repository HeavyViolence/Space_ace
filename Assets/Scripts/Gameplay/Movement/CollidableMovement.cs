using SpaceAce.Gameplay.Shooting;
using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    [RequireComponent(typeof(DamageDealer))]
    public abstract class CollidableMovement : Movement
    {
        [SerializeField] private MovementConfig _movementConfig = null;

        private DamageDealer _collisionDamageDealer = null;

        protected MovementConfig Config => _movementConfig;

        protected override void Awake()
        {
            base.Awake();

            _collisionDamageDealer = gameObject.GetComponent<DamageDealer>();
        }

        protected virtual void OnEnable()
        {
            if (Config.CollisionDamageEnabled)
            {
                _collisionDamageDealer.Hit += CollisionHitEventHandler;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Config.CollisionDamageEnabled)
            {
                _collisionDamageDealer.Hit -= CollisionHitEventHandler;
            }
        }

        protected virtual void CollisionHitEventHandler(object sender, HitEventArgs e)
        {
            e.DamageReceiver.ApplyDamage(Config.CollisionDamage);
            Config.CollisionAudio.PlayRandomAudioClip(e.HitPosition);

            if (Config.CameraShakeOnCollisionEnabled)
            {
                CameraShaker.Access.ShakeOnCollision();
            }
        }
    }
}