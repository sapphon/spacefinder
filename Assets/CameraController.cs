using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _aimPoint;
    private IEnumerator _currentCameraMovement;
    public float durationOfPan = 1;

    void Awake()
    {
        _camera = Camera.main;
        if (_camera == null)
        {
            Util.logIfDebugging("Main camera not found at CameraController awake!!");
        }
    }

    public static bool zoomCameraOut()
    {
        float previousSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = Mathf.Min(previousSize + 1, 22);
        return Camera.main.orthographicSize != previousSize;
    }

    public static bool zoomCameraIn()
    {
        float previousSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = Mathf.Max(previousSize - 1, 1);
        return Camera.main.orthographicSize != previousSize;
    }

    public void setAimPoint(Vector2 newAimPoint)
    {
        if (_currentCameraMovement != null)
        {
            StopCoroutine(_currentCameraMovement);
        }
        this._aimPoint = newAimPoint;
        _currentCameraMovement = lerpPosition(_aimPoint, durationOfPan);
        StartCoroutine(_currentCameraMovement);
    }
   

    IEnumerator lerpPosition(Vector2 targetXY, float duration)
    {
        float time = 0;
        Vector3 startPosition = _camera.transform.position;
        Vector3 targetPosition = new Vector3(targetXY.x, targetXY.y, startPosition.z);

        while (time < duration)
        {
            _camera.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _camera.transform.position = targetPosition;
    }
}
