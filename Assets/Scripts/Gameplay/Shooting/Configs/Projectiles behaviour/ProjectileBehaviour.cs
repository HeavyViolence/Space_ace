using SpaceAce.Gameplay.Movement;
using UnityEngine;

namespace SpaceAce.Gameplay.Shooting
{
    public abstract class ProjectileBehaviour : ScriptableObject
    {
        public abstract MovementBehaviour Behaviour { get; }
    }
}