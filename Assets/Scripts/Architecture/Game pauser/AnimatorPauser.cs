using UnityEngine;

namespace SpaceAce.Architecture
{
    public sealed class AnimatorPauser : MonoBehaviour, IPausable
    {
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        private Animator _animator;

        public bool Paused { get; private set; } = false;

        private void Awake()
        {
            _animator = gameObject.GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            s_gamePauser.Access.Register(this);
        }

        private void OnDisable()
        {
            s_gamePauser.Access.Deregister(this);
        }

        public void Pause()
        {
            if (Paused == true) return;

            _animator.speed = 0f;
            Paused = true;
        }

        public void Resume()
        {
            if (Paused == false) return;

            _animator.speed = 1f;
            Paused = false;
        }
    }
}