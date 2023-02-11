using SpaceAce.Architecture;
using UnityEngine;

namespace SpaceAce.Main
{
    public sealed class MasterCameraHolder : IInitializable
    {
        private const float CameraSize = 40f;

        public GameObject MasterObject { get; }
        public GameObject MasterCameraAnchor { get; }
        public Camera MasterCamera { get; }

        public Vector2 ViewportLowerLeftCornerWorldPosition { get; }
        public Vector2 ViewportUpperLeftCornerWorldPosition { get; }
        public Vector2 ViewportLowerRightCornerWorldPosition { get; }
        public Vector2 ViewportUpperRightCornerWorldPosition { get; }

        public float ViewportLeftBound => ViewportLowerLeftCornerWorldPosition.x;
        public float ViewportRightBound => ViewportLowerRightCornerWorldPosition.x;
        public float ViewportUpperBound => ViewportUpperLeftCornerWorldPosition.y;
        public float ViewportLowerBound => ViewportLowerLeftCornerWorldPosition.y;

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

            ViewportLowerLeftCornerWorldPosition = MasterCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
            ViewportUpperLeftCornerWorldPosition = MasterCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f));
            ViewportLowerRightCornerWorldPosition = MasterCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f));
            ViewportUpperRightCornerWorldPosition = MasterCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        }

        public bool InsideViewport(Vector2 position, float delta = 0f) => position.x + delta > ViewportLeftBound &&
                                                                          position.x - delta < ViewportRightBound &&
                                                                          position.y - delta < ViewportUpperBound &&
                                                                          position.y + delta > ViewportLowerBound;

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