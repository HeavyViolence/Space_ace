using SpaceAce.Architecture;
using System;
using UnityEngine;

namespace SpaceAce
{
    namespace Main
    {
        public sealed class MasterAudioListenerHolder: IInitializable
        {
            public AudioListener MasterAudioListener { get; }

            public MasterAudioListenerHolder(GameObject masterCameraAnchor)
            {
                if (masterCameraAnchor == null)
                {
                    throw new ArgumentNullException(nameof(masterCameraAnchor), "Attempted to pass an empty master camera anchor object!");
                }

                MasterAudioListener = masterCameraAnchor.AddComponent<AudioListener>();
            }

            #region interfaces

            public void OnInitialize()
            {
                GameServices.Register(this);
            }

            public void OnSubscribe()
            {

            }

            public void OnUnsubscribe()
            {

            }

            public void OnClear()
            {
                GameServices.Deregister(this);
            }

            #endregion
        }
    }
}