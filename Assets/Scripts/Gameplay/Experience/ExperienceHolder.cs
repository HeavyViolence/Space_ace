using SpaceAce.Architecture;
using SpaceAce.Main;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Experience
{
    public sealed class ExperienceHolder : MonoBehaviour
    {
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();

        [SerializeField] private ExperienceConfig _config;

        private IEnumerable<IExperienceSource> _experienceSources = null;
        private float _lifespanTimer;

        private void Awake()
        {
            _experienceSources = FindAllExperienceSources();
        }

        private void OnEnable()
        {
            _lifespanTimer = 0f;
        }

        private void Update()
        {
            if (_config.ExperienceDepletionEnabled &&
                s_masterCameraHolder.Access.InsideViewport(transform.position) == true &&
                _lifespanTimer < _config.ExperienceDepletionDuration)
            {
                _lifespanTimer += Time.deltaTime;
            }
        }

        private IEnumerable<IExperienceSource> FindAllExperienceSources()
        {
            List<IExperienceSource> sources = new();

            foreach (var source in transform.root.GetComponentsInChildren<IExperienceSource>())
            {
                sources.Add(source);
            }

            return sources;
        }

        private float GetEarnedExperienceFactor()
        {
            if (_config.ExperienceDepletionEnabled == false)
            {
                return 1f;
            }

            float clampedLifespanTimer = Mathf.Clamp(_lifespanTimer, 0f, _config.ExperienceDepletionDuration);

            return (_config.ExperienceDepletionDuration - _lifespanTimer) / _config.ExperienceDepletionDuration;
        }

        private float GetLostExperienceFactor()
        {
            if (_config.ExperienceDepletionEnabled == false)
            {
                return 0f;
            }

            return 1f - GetEarnedExperienceFactor();
        }

        public (float earned, float lost, float total) GetValues()
        {
            float totalValue = 0f;

            foreach (var source in _experienceSources)
            {
                totalValue += source.GetExperience();
            }

            float earnedExperience = totalValue * GetEarnedExperienceFactor();
            float lostExperience = totalValue * GetLostExperienceFactor();

            return (earnedExperience, lostExperience, totalValue);
        }
    }
}