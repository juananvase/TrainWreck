using UnityEngine;

public class GunOverheat : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MountedGun _mountedGun;
    
    private void Update()
    {
        float lastShotTime = _mountedGun.LastShotTime;
        float flashDuration = 0.25f;
        float timeSinceLast = Time.time - lastShotTime;
        timeSinceLast = Mathf.Clamp(timeSinceLast, 0f, flashDuration);
        float normalized = timeSinceLast / flashDuration;
        float intensity = 1f - normalized;
    
        float overheatIntensity = _mountedGun.OverheatAmount;
        intensity = Mathf.Max(intensity, overheatIntensity);
        
        
        SetMaterialValue("_Overheat", intensity);
        SetMaterialValue("_OverheatIntensity", 5f);
    
        if (_mountedGun.IsOverheating)
        {
            SetMaterialValue("_OverheatIntensity", 8f);
        }
    }
    
    private void SetMaterialValue(string materialName, float value)
    {
        _meshRenderer.material.SetFloat(materialName, value); 
    }
}
