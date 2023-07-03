using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Architecture
{
    public sealed class ParticleSystemPauser : MonoBehaviour, IPausable
    {
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        private readonly List<ParticleSystem> _particleSystems = new();

        public bool Paused { get; private set; } = false;

        private void Awake()
        {
            foreach (var system in gameObject.GetComponentsInChildren<ParticleSystem>()) _particleSystems.Add(system);
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

            foreach (var system in _particleSystems) if (system.isPlaying == true) system.Pause();
            Paused = true;
        }

        public void Resume()
        {
            if (Paused == false) return;

            foreach (var system in _particleSystems) if (system.isPaused == true) system.Play();
            Paused = false;
        }
    }
}