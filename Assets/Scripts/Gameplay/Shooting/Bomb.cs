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

            CoroutineRunner.RunRoutine(DamageablesDestructionRoutine(hits));
        }

        private IEnumerator DamageablesDestructionRoutine(IEnumerable<RaycastHit2D> hits)
        {
            foreach (var hit in hits)
            {
                yield return new WaitForSeconds(NextDamageDelay);

                if (s_masterCameraHolder.Access.InsideViewport(hit.transform.position) == true &&
                    hit.transform.gameObject.TryGetComponent(out IDamageable damageable) == true)
                {
                    damageable.ApplyDamage(NextExplosionDamage);
                }
            }
        }
    }
}