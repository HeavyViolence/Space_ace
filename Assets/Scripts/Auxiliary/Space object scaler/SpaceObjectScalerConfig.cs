using UnityEngine;

namespace SpaceAce.Auxiliary
{
    [CreateAssetMenu(fileName = "Space object scaler config", menuName = "Space ace/Configs/Space object scaler config")]
    public sealed class SpaceObjectScalerConfig : ScriptableObject
    {
        public const float MinScale = 0.1f;
        public const float MaxScale = 10f;

        [SerializeField] private float _minScale = MinScale;
        [SerializeField] private float _maxScale = MaxScale;

        public Vector3 MinTargetScale => new(_minScale, _minScale, 1f);
        public Vector3 MaxTargetScale => new(_maxScale, _maxScale, 1f);
        public Vector3 RandomTargetScale => new(Random.Range(_minScale, _maxScale), Random.Range(_minScale, _maxScale), 1f);
    }
}