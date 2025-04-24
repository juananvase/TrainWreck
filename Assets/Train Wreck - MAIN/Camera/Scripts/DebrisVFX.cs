using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Density/Turbulence")]
public class DebrisVFX : VFXBinderBase
{
    private ExposedProperty _density;
    private ExposedProperty _turbulence;

    [SerializeField] private float _targetDensity;
    [SerializeField] private float _targetTurbulence;

    public override bool IsValid(VisualEffect component)
    {
        return component.HasFloat(_density) && component.HasFloat(_turbulence);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        
    }
}
