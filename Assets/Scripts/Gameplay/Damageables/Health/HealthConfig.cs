using SpaceAce.Auxiliary;
using SpaceAce.Main.Audio;
using SpaceAce.Main.ObjectPooling;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [CreateAssetMenu(fileName = "Health Config", menuName = "Space ace/Configs/Damageables/Health Config")]
    public sealed class HealthConfig : ScriptableObject
    {
        public const float MinHealth = 100f;
        public const float MaxHealth = 10000f;

        public const float MinHealthRegenPerSecond = 10f;
        public const float MaxHealthRegenPerSecond = 1000f;

        [SerializeField] private float _healthLimit = MinHealth;
        [SerializeField] private float _healthLimitRandomDeviation = 0f;

        [SerializeField] private bool _regenEnabled = false;
        [SerializeField] private float _regenPerSecond = MinHealthRegenPerSecond;
        [SerializeField] private float _regenPerSecondRandomDeviation = 0f;

        [SerializeField] private ObjectPoolEntry _deathEffect = null;
        [SerializeField] private AudioCollection _deathAudio = null;

        [SerializeField] private bool _cameraShakeOnDamagedEnabled = false;

        public float RandomHealthLimit => _healthLimit + _healthLimitRandomDeviation * AuxMath.RandomNormal;
        public float MinHealthLimit => _healthLimit - _healthLimitRandomDeviation;
        public float MaxHealthLimit => _healthLimit + _healthLimitRandomDeviation;

        public bool RegenEnabled => _regenEnabled;
        public float MinRegenPerSecond => RegenEnabled ? _regenPerSecond - _regenPerSecondRandomDeviation : 0f;
        public float MaxRegenPerSecond => RegenEnabled ? _regenPerSecond + _regenPerSecondRandomDeviation : 0f;
        public float RandomRegenPerSecond => RegenEnabled ? _regenPerSecond + _regenPerSecondRandomDeviation * AuxMath.RandomNormal : 0f;

        public string DeathEffectAnchorName => _deathEffect.AnchorName;

        public AudioCollection DeathAudio => _deathAudio;

        public bool CameraShakeOnDamagedEnabled => _cameraShakeOnDamagedEnabled;

        public void EnsureDeathEffectObjectPoolExistence() => _deathEffect.EnsureObjectPoolExistence();
    }
}