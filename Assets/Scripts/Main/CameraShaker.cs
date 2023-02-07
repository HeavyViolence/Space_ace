using SpaceAce.Architecture;
using SpaceAce.Auxiliary;
using SpaceAce.Main.Saving;
using System;
using System.Collections;
using UnityEngine;

namespace SpaceAce.Main
{
    public sealed class CameraShaker : IInitializable, ISavable
    {
        private const float MaxAmplitude = 1f;
        private const float MaxAttenuation = 2f;
        private const float MaxFrequency = 10f;
        private const float Cutoff = 0.01f;

        public event EventHandler SavingRequested;

        private Rigidbody2D _body;
        private int _activeShakers = 0;
        private bool _shakingEnabled = true;

        public bool ShakingEnabled
        {
            get
            {
                return _shakingEnabled;
            }
            set
            {
                _shakingEnabled = value;
                SavingRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public string ID { get; }

        public CameraShaker(string id, GameObject masterCameraAnchor)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("Attempted to pass an invalid ID!");
            }

            ID = id;

            if (masterCameraAnchor == null)
            {
                throw new ArgumentNullException(nameof(masterCameraAnchor), "Attempted to pass an empty master camera anchor!");
            }

            _body = masterCameraAnchor.AddComponent<Rigidbody2D>();

            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.simulated = true;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            _body.useFullKinematicContacts = false;
            _body.sleepMode = RigidbodySleepMode2D.StartAwake;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void ShakeOnShotFired() => Shake(0.02f, 2f, 2f);

        public void ShakeOnDeath() => Shake(0.2f, 2f, 1f);

        public void ShakeOnCollision() => Shake(0.1f, 2f, 2f);

        public void ShakeOnHit() => Shake(0.05f, 2f, 2f);

        public void Shake(float amplitude, float attenuation, float frequency)
        {
            if (ShakingEnabled)
            {
                CoroutineRunner.RunRoutine(ShakeOnce(amplitude, attenuation, frequency));
            }
        }

        private IEnumerator ShakeOnce(float amplitude, float attenuation, float frequency)
        {
            _activeShakers++;

            amplitude = Mathf.Clamp(amplitude, 0f, MaxAmplitude);
            attenuation = Mathf.Clamp(attenuation, 0f, MaxAttenuation);
            frequency = Mathf.Clamp(frequency, 0f, MaxFrequency);

            float timer = 0f;
            float duration = -1f * Mathf.Log(Cutoff, (float)Math.E) / attenuation;

            while (timer < duration)
            {
                timer += Time.fixedDeltaTime;

                float delta = amplitude * Mathf.Exp(-1f * attenuation * timer) * Mathf.Sin(2f * Mathf.PI * frequency * timer);
                float deltaX = delta * AuxMath.RandomSign;
                float deltaY = delta * AuxMath.RandomSign;
                var deltaPos = new Vector2(deltaX, deltaY);

                _body.MovePosition(deltaPos);

                yield return new WaitForFixedUpdate();
            }

            if (--_activeShakers == 0)
            {
                _body.MovePosition(Vector2.zero);
            }
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Register(this);
            }
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out SavingSystem system) == true)
            {
                system.Deregister(this);
            }
        }

        public void OnClear()
        {
            GameServices.Deregister(this);
        }

        public object GetState() => ShakingEnabled;

        public void SetState(object state)
        {
            if (state is bool value)
            {
                ShakingEnabled = value;
            }
            else
            {
                throw new LoadedSavableEntityStateTypeMismatchException(state.GetType(), typeof(bool), GetType());
            }
        }

        public override bool Equals(object obj) => Equals(obj as ISavable);

        public bool Equals(ISavable other) => other is not null && other.ID.Equals(ID);

        public override int GetHashCode() => ID.GetHashCode();

        #endregion
    }
}