using SpaceAce.Auxiliary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceAce.Gameplay.Amplifications
{
    public sealed class Amplifier : MonoBehaviour
    {
        private IEnumerable<IAmplifiable> _amplifiables;
        private Animator _animator;

        public bool Active { get; private set; } = false;

        private void Awake()
        {
            _amplifiables = gameObject.GetComponentsInChildren<IAmplifiable>();

            if (_amplifiables.Count() == 0) throw new MissingComponentException(typeof(IAmplifiable).ToString());

            _animator = gameObject.transform.root.GetComponentInChildren<Animator>();

            if (_animator == null) throw new MissingComponentException(typeof(Animator).ToString());
        }

        private void OnDisable()
        {
            _animator.SetTrigger("Deamplified");
            Active = false;
        }

        public void Amplify(RangedFloat factor)
        {
            foreach (var amplifiable in _amplifiables) amplifiable.Amplify(factor.RandomValue);

            _animator.SetTrigger("Amplified");
            Active = true;
        }
    }
}