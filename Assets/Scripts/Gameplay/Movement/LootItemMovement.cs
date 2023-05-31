using UnityEngine;

namespace SpaceAce.Gameplay.Movement
{
    public sealed class LootItemMovement : SpaceObjectMovement
    {
        protected override Vector3 GetMovementDirection()
        {
            float targetPositionX = transform.position.x;
            float targetPositionY = MasterCameraHolder.Access.ViewportLowerBound;

            Vector3 targetPosition = new(targetPositionX, targetPositionY, 0f);

            return (targetPosition - transform.position).normalized;
        }
    }
}