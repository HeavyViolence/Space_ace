using Newtonsoft.Json;
using SpaceAce.Gameplay.Movement;
using SpaceAce.Gameplay.Shooting;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAce.Gameplay.Inventories
{
    public sealed class HomingAmmo : InventoryItem, IEquatable<HomingAmmo>
    {
        public const float MinHomingSpeed = 15f;
        public const float MaxHomingSpeed = 120f;

        [JsonIgnore]
        public override string Title => throw new NotImplementedException();

        [JsonIgnore]
        public override string Description => throw new NotImplementedException();

        [JsonIgnore]
        public override string Stats => throw new NotImplementedException();

        [JsonIgnore]
        public override float Worth => (base.Worth + HomingSpeed * AmmoHomingSpeedUnitWorth) * (float)(Rarity + 1);

        public float HomingSpeed { get; }

        public HomingAmmo(ItemRarity rarity, float duration, float homingSpeed) : base(rarity, duration)
        {
            HomingSpeed = Mathf.Clamp(homingSpeed, MinHomingSpeed, MaxHomingSpeed);
        }

        public Func<Vector2, Transform> TargetLocator => delegate(Vector2 origin)
        {
            int layerMask = LayerMask.GetMask("Bosses", "Enemies", "Meteors", "Space debris");
            var hit = Physics2D.CircleCast(origin, TargetSupplier.SearchWidth, Vector2.up, float.PositiveInfinity, layerMask);

            return hit.collider == null ? null : hit.transform;
        };

        public MovementBehaviour AmmoBehaviour => delegate(Rigidbody2D body, MovementBehaviourSettings settings, ref MovementAuxiliaryData data)
        {
            data.Timer += Time.fixedDeltaTime;

            float speedFactor = Mathf.Clamp01(data.Timer / settings.TopSpeedGainDuration);
            float speed = settings.TopSpeed * speedFactor;
            Vector2 velocity;

            if (data.CurrentDirection == Vector3.zero) data.CurrentDirection = settings.InitialDirection;

            if (settings.Target == null)
            {
                velocity = Time.fixedDeltaTime * speed * settings.InitialDirection;
                body.MovePosition(body.position + velocity);
            }
            else
            {
                if (data.CurrentRotation == Quaternion.identity)
                    data.CurrentRotation = Quaternion.LookRotation(Vector3.forward, settings.InitialDirection);

                if (settings.Target.gameObject.activeInHierarchy == true)
                {
                    Vector3 targetDirection = (settings.Target.position - body.transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

                    data.CurrentRotation = Quaternion.RotateTowards(data.CurrentRotation, targetRotation, settings.TargetSeekingSpeed * Time.fixedDeltaTime);
                    data.CurrentDirection = Vector3.RotateTowards(data.CurrentDirection,
                                                                  targetDirection,
                                                                  settings.TargetSeekingSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,
                                                                  1f).normalized;

                    velocity = Time.fixedDeltaTime * speed * data.CurrentDirection;

                    body.MovePosition(body.position + velocity);
                    body.MoveRotation(data.CurrentRotation);
                }
                else
                {
                    velocity = Time.fixedDeltaTime * speed * data.CurrentDirection;
                    body.MovePosition(body.position + velocity);
                }
            }
        };

        public override bool Fuse(InventoryItem item1, InventoryItem item2, out InventoryItem result)
        {
            if (item1 is not null &&
                item2 is not null &&
                Rarity.Equals(item1.Rarity) &&
                Rarity.Equals(item2.Rarity) &&
                item1 is HomingAmmo other1 &&
                item2 is HomingAmmo other2)
            {
                ItemRarity nextRarity = GetNextRarity(Rarity);
                float newDuration = (Duration + other1.Duration + other2.Duration) * FusedPropertyFactor;
                float newHomingSpeed = (HomingSpeed + other1.HomingSpeed + other2.HomingSpeed) * FusedPropertyFactor;

                result = new HomingAmmo(nextRarity, newDuration, newHomingSpeed);
                return true;
            }

            result = null;
            return false;
        }

        public override bool Use()
        {
            if (GameModeLoader.Access.GameState == Main.GameState.Level &&
                SpecialEffectsMediator.Access.TryGetEffectReceivers(out IEnumerable<IHomingAmmoUser> users) == true)
            {
                bool used = false;

                foreach (var user in users) if (user.Use(this) == true) used = true;

                if (used)
                {
                    HUDDisplay.Access.RegisterActiveItem(this);
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as InventoryItem);

        public bool Equals(HomingAmmo other) => base.Equals(other) && HomingSpeed.Equals(other.HomingSpeed);

        public override int GetHashCode() => base.GetHashCode() ^ HomingSpeed.GetHashCode();
    }
}