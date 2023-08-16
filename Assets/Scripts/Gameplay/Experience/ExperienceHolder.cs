using SpaceAce.Architecture;
using SpaceAce.Gameplay.Inventories;
using SpaceAce.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Experience
{
    public sealed class ExperienceHolder : MonoBehaviour, IAdvancedLearningUser
    {
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();
        private static readonly GameServiceFastAccess<SpecialEffectsMediator> s_specialEffectsMediator = new();

        [SerializeField] private ExperienceConfig _config;

        [SerializeField] private bool _experienceDisabled = false;

        private IEnumerable<IExperienceSource> _experienceSources = null;
        private float _lifespanTimer;

        private Coroutine _advancedLearning = null;
        private float _experienceBoost = 1f;
        private float _experienceDepletionSlowdown = 1f;

        private void Awake()
        {
            if (_experienceDisabled == true) return;
            
            _experienceSources = FindAllExperienceSources();
        }

        private void OnEnable()
        {
            s_specialEffectsMediator.Access.Register(this);

            _lifespanTimer = 0f;
        }

        private void OnDisable()
        {
            s_specialEffectsMediator.Access.Deregister(this);

            if (_advancedLearning != null)
            {
                StopCoroutine(_advancedLearning);
                _advancedLearning = null;
                _experienceBoost = 1f;
                _experienceDepletionSlowdown = 1f;
            }
        }

        private void Update()
        {
            if (_experienceDisabled == false &&
                _config.ExperienceDepletionEnabled &&
                s_masterCameraHolder.Access.InsideViewport(transform.position) == true &&
                _lifespanTimer < _config.ExperienceDepletionDuration &&
                s_gamePauser.Access.Paused == false)
            {
                _lifespanTimer += Time.deltaTime;
            }
        }

        private IEnumerable<IExperienceSource> FindAllExperienceSources()
        {
            List<IExperienceSource> sources = new();

            foreach (var source in transform.root.GetComponentsInChildren<IExperienceSource>()) sources.Add(source);

            return sources;
        }

        private float GetEarnedExperienceFactor()
        {
            if (_config.ExperienceDepletionEnabled == false) return 1f;

            return (_config.ExperienceDepletionDuration * _experienceDepletionSlowdown - _lifespanTimer) /
                   (_config.ExperienceDepletionDuration * _experienceDepletionSlowdown);
        }

        private float GetLostExperienceFactor()
        {
            if (_config.ExperienceDepletionEnabled == false) return 0f;

            return 1f - GetEarnedExperienceFactor();
        }

        public (float earned, float lost, float total) GetValues()
        {
            if (_experienceDisabled) return (0f, 0f, 0f);

            float totalValue = 0f;

            foreach (var source in _experienceSources) totalValue += source.GetExperience();

            if (_experienceBoost != 1f) totalValue *= _experienceBoost;

            float earnedExperience = totalValue * GetEarnedExperienceFactor();
            float lostExperience = totalValue * GetLostExperienceFactor();

            return (earnedExperience, lostExperience, totalValue);
        }

        public bool Use(AdvancedLearning learning)
        {
            if (learning is null) throw new ArgumentNullException(nameof(learning));

            if (_advancedLearning == null)
            {
                _advancedLearning = StartCoroutine(ApplyAdvancedLearning(learning));
                return true;
            }

            return false;
        }

        private IEnumerator ApplyAdvancedLearning(AdvancedLearning learning)
        {
            _experienceBoost = learning.ExperienceBoost;
            _experienceDepletionSlowdown = learning.ExperienceDepletionSlowdown;
            float timer = 0f;

            while (timer < learning.Duration)
            {
                timer += Time.deltaTime;

                yield return null;
                while (s_gamePauser.Access.Paused == true) yield return null;
            }

            _experienceBoost = 1f;
            _experienceDepletionSlowdown = 1f;
            _advancedLearning = null;
        }
    }
}