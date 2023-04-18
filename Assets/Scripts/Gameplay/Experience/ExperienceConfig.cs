using UnityEngine;

namespace SpaceAce.Gameplay.Experience
{
    [CreateAssetMenu(fileName = "Experience config", menuName = "Space ace/Configs/Damageables/Experience config")]
    public sealed class ExperienceConfig : ScriptableObject
    {
        public const float MinExperienceDepletionDuration = 10f;
        public const float MaxExperienceDepletionDuration = 60f;

        [SerializeField] private bool _experienceDepletionEnabled = false;

        [SerializeField] private float _experienceDepletionPeriod = MinExperienceDepletionDuration;
        [SerializeField] private AnimationCurve _experienceDepletionCurve;

        public bool ExperienceDepletionEnabled => _experienceDepletionEnabled;
        public float ExperienceDepletionDuration => ExperienceDepletionEnabled ? _experienceDepletionPeriod : Mathf.Infinity;

        public float GetExperienceFactor(float lifespan)
        {
            if (ExperienceDepletionEnabled == false)
            {
                return 1f;
            }

            float clampedLifespan = Mathf.Clamp(lifespan, 0f, ExperienceDepletionDuration);
            float evaluator = (ExperienceDepletionDuration - clampedLifespan) / ExperienceDepletionDuration;

            return _experienceDepletionCurve.Evaluate(evaluator);
        }
    }
}