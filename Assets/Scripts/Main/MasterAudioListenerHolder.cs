using SpaceAce.Architecture;
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