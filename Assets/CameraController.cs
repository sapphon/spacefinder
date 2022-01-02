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
    }


    public void SetAimPoint(Vector2 newAimPoint)
    {
        if (_currentCameraMovement != null)
        {
            StopCoroutine(_currentCameraMovement);
        }
        this._aimPoint = newAimPoint;
        _currentCameraMovement = LerpPosition(_aimPoint, durationOfPan);
        Debug.Log("starting coroutine");
        StartCoroutine(_currentCameraMovement);
    }
   

    IEnumerator LerpPosition(Vector2 targetXY, float duration)
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
