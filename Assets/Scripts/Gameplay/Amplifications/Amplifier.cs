using SpaceAce.Auxiliary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceAce.Gameplay.Amplifications
{
    public sealed class Amplifier : MonoBehaviour
    {
        private const string AnimatorAmplifiedStateTrigger = "Amplified";
        private const string AnimatorDeamplifiedStateTrigger = "Deamplified";

        private IEnumerable<IAmplifiable> _amplifiables;
        private Animator _animator;

        private void Awake()
        {
            _amplifiables = gameObject.GetComponentsInChildren<IAmplifiable>();

            if (_amplifiables.Count() == 0)
            {
                throw new MissingComponentException($"Amplifiable entity is missing at least one component of type {typeof(IAmplifiable)}!");
            }

            _animator = gameObject.transform.root.GetComponentInChildren<Animator>();

            if (_animator == null)
            {
                throw new MissingComponentException($"Amplifiable entity is missing a mandatory component of type {typeof(Animator)}!");
            }
        }

        private void OnDisable()
        {
            _animator.SetTrigger(AnimatorDeamplifiedStateTrigger);
        }

        public void Amplify(RangedFloat factor)
        {
            foreach (var amplifiable in _amplifiables)
            {
                amplifiable.Amplify(factor.RandomValue);
            }

            _animator.SetTrigger(AnimatorAmplifiedStateTrigger);
        }
    }
}