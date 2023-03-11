using UnityEngine;

namespace SpaceAce.Gameplay.Players
{
    public interface IPlayerShipMovementController
    {
        void Move(Vector2 direction);
    }
}