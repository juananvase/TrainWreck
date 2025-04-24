 using UnityEngine;
using UnityEngine.Splines;

public class VertexBulge : MonoBehaviour
{
    [field: SerializeField] public SplineContainer Spline { get; private set; }
    [field: SerializeField] public MeshRenderer[] MeshRenderers { get; private set; }
    [field: SerializeField] public Health[] PipeHealth { get; private set; }

    [SerializeField] private float _speed;
    
    private float _progress;
    private Vector3 _pos;

    private void OnValidate()
    {
        Spline = GetComponentInChildren<SplineContainer>();
        MeshRenderers = GetComponentsInChildren<MeshRenderer>();
        PipeHealth = GetComponentsInChildren<Health>();
        
        if (MeshRenderers.Length == 0)
        {
            return;
        }

        foreach (var meshes in MeshRenderers)
        {
            if (meshes.TryGetComponent(out Health pipeHealth))
            {
                for (int i = 0; i < PipeHealth.Length; i++)
                {
                    PipeHealth[i] = pipeHealth;
                }
            }
        }
    }

    private void Update()
    {
      foreach (var health in PipeHealth)
      {
          if (health.IsAlive == false)
          {
              return;
          }
      }

      _progress += Time.deltaTime * _speed;
      if(_progress >= 1)
      {
          _progress = 0;
      }
      _pos = Spline.EvaluatePosition(_progress);
      _pos = Spline.transform.TransformVector(_pos);
            
      SetMaterialValue("_BulgePosition", _pos);
    }

    private void SetMaterialValue(string materialName, Vector3 value)
    {
        if (MeshRenderers == null || MeshRenderers.Length == 0)
        {
            return;
        }
        foreach (var meshRenderer in MeshRenderers)
        {
            meshRenderer.material.SetVector(materialName, value);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_pos, 0.5f);
    }
}
