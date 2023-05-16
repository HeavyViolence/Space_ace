using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class MovementAuxiliaryData
    {
        public float Timer { get; set; }
        public Vector3 CurrentDirection { get; set; }
        public Quaternion CurrentRotation { get; set; }

        public MovementAuxiliaryData()
        {
            Reset();
        }

        public void Reset()
        {
            Timer = 0f;
            CurrentDirection = Vector3.zero;
            CurrentRotation = Quaternion.identity;
        }
    }
}