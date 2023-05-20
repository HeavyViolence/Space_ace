using UnityEngine;

namespace SpaceAce.Auxiliary
{
    public sealed class SpaceObjectScaler : MonoBehaviour
    {
        [SerializeField] private SpaceObjectScalerConfig _config;

        private void OnEnable()
        {
            transform.localScale = _config.RandomTargetScale;
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }
    }
}