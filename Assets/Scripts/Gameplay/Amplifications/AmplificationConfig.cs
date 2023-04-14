using SpaceAce.Auxiliary;
using SpaceAce.Main.Audio;
using UnityEngine;

namespace SpaceAce.Gameplay.Amplifications
{
    [CreateAssetMenu(fileName = "Amplification config", menuName = "Space ace/Configs/Amplification config")]
    public sealed class AmplificationConfig : ScriptableObject
    {
        public const float MinAplificationFactor = 1f;
        public const float MaxAmplificationFactor = 3f;

        public const float MinAmplificationProbabiltiy = 0.01f;
        public const float MaxAmplificationProbability = 0.1f;

        [SerializeField] private AudioCollection _amplifiedEntitySpawnAudio;

        [SerializeField] private float _amplificationFactor = MinAplificationFactor;
        [SerializeField] private float _amplificationFactorRandomDeviation = 0f;

        [SerializeField] private float _amplificationProbability = MinAmplificationProbabiltiy;
        [SerializeField] private float _amplificationProbabilityRandomDeviation = 0f;

        public AudioCollection AmplifiedEntitySpawnAudio => _amplifiedEntitySpawnAudio;
        public RangedFloat AmplificationFactor { get; private set; }
        public RangedFloat AmplificationProbability { get; private set; }

        private void OnEnable()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            AmplificationFactor = new(_amplificationFactor,
                                      _amplificationFactorRandomDeviation,
                                      MinAplificationFactor,
                                      MaxAmplificationFactor * 2f);

            AmplificationProbability = new(_amplificationProbability,
                                           _amplificationProbabilityRandomDeviation,
                                           MinAmplificationProbabiltiy,
                                           MaxAmplificationProbability * 2f);
        }
    }
}