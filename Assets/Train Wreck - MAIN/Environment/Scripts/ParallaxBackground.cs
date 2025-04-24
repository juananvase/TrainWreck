using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private static readonly int BaseOffset = Shader.PropertyToID("_Base_Offset");

    [Header("Components")]
    [field: SerializeField] private MeshRenderer Renderer;
    [SerializeField] private Transform _killVolumeLoc;
    
    [Header("Data's")]
    [SerializeField] public CameraDataSO _cameraData;
    [SerializeField] public TrainDataSO _trainData;
    
    [Header("Speed Settings")]
    [SerializeField] private float _speedRate = 100f;
    [SerializeField] private float _speedMultiplier;

    private float _distance = 0;

    private void Update()
    {
        if (_cameraData == null)
        {
            Debug.LogError("CameraData is null");
            return;
        }
        CheckSpeed();
    }
    
    private void CheckSpeed()
    {
        float speed = _trainData.CurrentTrainSpeed / _speedRate * _speedMultiplier;
        _distance += Time.deltaTime * speed;
        
        float smoothedDistance = Mathf.Lerp(Renderer.sharedMaterial.GetVector(BaseOffset).x, _distance, 0.1f);
        Renderer.material.SetVector(BaseOffset, new Vector2(smoothedDistance, 0f));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_killVolumeLoc == null)
        {
            Debug.LogError("_killVolumeLoc is null");
            return;
        }
        
        if (other.collider.TryGetComponent(out IRespawnable respawnable))
        {
            respawnable.Respawn(_killVolumeLoc, _trainData.CurrentTrainSpeed);
        }
    }
}