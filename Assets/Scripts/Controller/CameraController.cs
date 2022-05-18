using UnityEngine;

namespace Controller
{
    public class CameraController
    {
        public static bool ZoomCameraOut()
        {
            float previousSize = Camera.main.orthographicSize;
            Camera.main.orthographicSize = Mathf.Min(previousSize + 1, 22);
            return Camera.main.orthographicSize != previousSize;
        }

        public static bool ZoomCameraIn()
        {
            float previousSize = Camera.main.orthographicSize;
            Camera.main.orthographicSize = Mathf.Max(previousSize - 1, 1);
            return Camera.main.orthographicSize != previousSize;
        }
    }
}