using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class TargetSupplier : ScriptableObject
    {
        public const float SearchWidth = 5f;

        public abstract Transform GetTarget(Vector2 origin);
    }
}