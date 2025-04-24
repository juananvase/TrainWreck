using System.Collections;
using GameEvents;
using Unity.Cinemachine;
using UnityEngine;

public class CamTransitions : MonoBehaviour
{
    [SerializeField] private CinemachineFollow _camera;
    [SerializeField] private CameraDataSO _cameraData;
    [SerializeField] private BoolEventAsset OnBeginGameplay;
    
    private void Start()
    {
        if (_camera == null || _cameraData == null)
        {
            return;
        }
        _camera.FollowOffset = _cameraData.LostPosition;
        StartCoroutine(StartTransition(_cameraData.MiddlePosition, _cameraData.CamPosChangeTime));
    }
    
    private IEnumerator StartTransition(Vector3 newPosition, float camPosChangeTime)
    {
        if (_camera == null)
        {
            yield break;
        }
        float elapsedTime = 0;
        Vector3 startPos = _camera.FollowOffset;
        while (elapsedTime < camPosChangeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / camPosChangeTime);
            _camera.FollowOffset = Vector3.Lerp(startPos, newPosition,  t);
            yield return null;
        }
        _camera.FollowOffset = newPosition;
        OnBeginGameplay.Invoke(true);
    }
}
