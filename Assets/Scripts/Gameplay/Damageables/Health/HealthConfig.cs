using SpaceAce.Auxiliary;
using SpaceAce.Main.Audio;
using SpaceAce.Main.ObjectPooling;
using UnityEngine;

namespace SpaceAce.Gameplay.Damageables
{
    [CreateAssetMenu(fileName = "Health config", menuName = "Space ace/Configs/Damageables/Health config")]
    public sealed class HealthConfig : ScriptableObject
    {
        public const float MinHealth = 10f;
        public const float MaxHealth = 100000f;

        public const float MinHealthRegenerationPerSecond = 10f;
        public const float MaxHealthRegenerationPerSecond = 1000f;

        [SerializeField] private float _maxHealth = MinHealth;
        [SerializeField] private float _maxHealthRandomDeviation = 0f;

        [SerializeField] private bool _regenerationEnabled = false;
        [SerializeField] private float _regenerationPerSecond = MinHealthRegenerationPerSecond;
        [SerializeField] private float _regenerationPerSecondRandomDeviation = 0f;

        [SerializeField] private ObjectPoolEntry _deathEffect = null;
        [SerializeField] private AudioCollection _deathAudio = null;

        [SerializeField] private bool _cameraShakeOnDamaged = false;

        public RangedFloat HealthCeiling { get; private set; }

        public bool RegenerationEnabled => _regenerationEnabled;
        public RangedFloat Regeneration { get; private set; }

        public string DeathEffectAnchorName => _deathEffect.AnchorName;

        public AudioCollection DeathAudio => _deathAudio;

        public bool CameraShakeOnDamagedEnabled => _cameraShakeOnDamaged;

        private void OnEnable()
        {
            ApplySettings();
        }

        public void EnsureDeathEffectObjectPoolExistence() => _deathEffect.EnsureObjectPoolExistence();

        public void ApplySettings()
        {
            HealthCeiling = new(_maxHealth, _maxHealthRandomDeviation);
            Regeneration = RegenerationEnabled ? new(_regenerationPerSecond, _regenerationPerSecondRandomDeviation) : RangedFloat.Zero;
        }
    }
}