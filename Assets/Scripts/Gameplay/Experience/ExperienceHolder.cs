using SpaceAce.Architecture;
using SpaceAce.Main;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Experience
{
    public sealed class ExperienceHolder : MonoBehaviour
    {
        private static readonly GameServiceFastAccess<MasterCameraHolder> s_masterCameraHolder = new();
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        [SerializeField] private ExperienceConfig _config;

        [SerializeField] private bool _experienceDisabled = false;

        private IEnumerable<IExperienceSource> _experienceSources = null;
        private float _lifespanTimer;

        private void Awake()
        {
            if (_experienceDisabled == true) return;
            
            _experienceSources = FindAllExperienceSources();
        }

        private void OnEnable()
        {
            _lifespanTimer = 0f;
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

            return (_config.ExperienceDepletionDuration - _lifespanTimer) / _config.ExperienceDepletionDuration;
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

            float earnedExperience = totalValue * GetEarnedExperienceFactor();
            float lostExperience = totalValue * GetLostExperienceFactor();

            return (earnedExperience, lostExperience, totalValue);
        }
    }
}