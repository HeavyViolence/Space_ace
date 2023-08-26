using SpaceAce.Architecture;
using SpaceAce.Gameplay.Damageables;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    [RequireComponent(typeof(BombDamageReceiver))]
    [RequireComponent(typeof(BombMovement))]
    public sealed class Bomb : MonoBehaviour
    {
        [SerializeField] private BombConfig _config;

        private IDestroyable _bombDestroyable;

        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<GamePauser> s_gamePuaser = new();
        private static readonly GameServiceFastAccess<GameModeLoader> s_gameModeLoader = new();

        private float NextExplosionDamage => _config.Damage.RandomValue;
        private float NextDamageDelay => _config.DamageDelay.RandomValue;

        private void Awake()
        {
            _bombDestroyable = gameObject.GetComponent<IDestroyable>();
        }

        private void OnEnable()
        {
            _bombDestroyable.BeforeDestroyed += ExplodeBomb;
        }

        private void OnDisable()
        {
            _bombDestroyable.BeforeDestroyed -= ExplodeBomb;
        }

        private void ExplodeBomb(object sender, System.EventArgs e)
        {
            var hits = Physics2D.CircleCastAll(transform.position,
                                               float.PositiveInfinity,
                                               Vector2.down,
                                               float.PositiveInfinity,
                                               LayerMask.GetMask("Player", "Enemies", "Bosses", "Meteors", "Space debris"));

            CoroutineRunner.RunRoutine(DamageEntities(hits));
        }

        private IEnumerator DamageEntities(IEnumerable<RaycastHit2D> hits)
        {
            float timer;
            float delay;

            foreach (var hit in hits)
            {
                timer = 0f;
                delay = NextDamageDelay;

                while (timer < delay)
                {
                    timer += Time.deltaTime;

                    if (s_gameModeLoader.Access.GameMode != GameMode.Level) yield break;

                    yield return null;
                    if (s_gamePuaser.Access.Paused == true) yield return null;
                }

                if (s_masterCameraHolder.Access.InsideViewport(hit.transform.position) == true &&
                    hit.transform.gameObject.TryGetComponent(out IDamageable damageable) == true)
                {
                    damageable.ApplyDamage(NextExplosionDamage);
                }
            }
        }
    }
}