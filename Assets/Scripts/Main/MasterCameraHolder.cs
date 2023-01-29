using SpaceAce.Architecture;
using UnityEngine;

namespace SpaceAce
{
    namespace Main
    {
        public sealed class MasterCameraHolder : IInitializable
        {
            private const float CameraSize = 40f;

            public GameObject MasterObject { get; }
            public GameObject MasterCameraAnchor { get; }
            public Camera MasterCamera { get; }

            public float LeftViewportBound { get; }
            public float RightViewportBound { get; }
            public float UpperViewportBound { get; }
            public float LowerViewportBound { get; }

            public MasterCameraHolder()
            {
                MasterObject = new("Master camera holder");

                MasterCameraAnchor = new("Master camera anchor");
                MasterCameraAnchor.transform.parent = MasterObject.transform;

                MasterCamera = MasterCameraAnchor.AddComponent<Camera>();
                MasterCamera.transform.parent = MasterObject.transform;
                MasterCamera.orthographic = true;
                MasterCamera.orthographicSize = CameraSize;
                MasterCamera.transform.position = new Vector3(0f, 0f, -1f);

                LeftViewportBound = MasterCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
                RightViewportBound = MasterCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
                UpperViewportBound = MasterCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
                LowerViewportBound = MasterCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;
            }

            public bool InsideViewport(Vector2 position, float delta = 0f) => position.x + delta > LeftViewportBound &&
                                                                              position.x - delta < RightViewportBound &&
                                                                              position.y - delta < UpperViewportBound &&
                                                                              position.y + delta > LowerViewportBound;
            
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