using System.Collections;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [RequireComponent(typeof(BombHealth))]
    [RequireComponent(typeof(BombArmor))]
    public sealed class BombDamageReceiver : DamageReceiver
    {
        private const float InvulnerabilityPeriod = 3f;

        private bool _invulnerable;

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(AwaitInvulnerabilityPeriod());
        }

        private IEnumerator AwaitInvulnerabilityPeriod()
        {
            _invulnerable = true;

            while (MasterCameraHolder.Access.InsideViewport(transform.position) == false) yield return null;

            float timer = 0f;

            while (timer < InvulnerabilityPeriod)
            {
                timer += Time.deltaTime;

                yield return null;
                while (GamePauser.Access.Paused == true) yield return null;
            }

            _invulnerable = false;
        }

        public override void ApplyDamage(float damage)
        {
            if (_invulnerable) return;

            base.ApplyDamage(damage);
        }
    }
}