using UnityEngine;
using UnityEngine.VFX;

public class Debris : MonoBehaviour
{
    [SerializeField] private VisualEffect _debrisEffect;
    [SerializeField] private TrainDataSO _trainData;
    [SerializeField] private float _minimumDebris;
    [SerializeField] private float _normalDebris;
    [SerializeField] private float _higherDebris;

    private void Update()
    {
        if (!(_trainData.CurrentTrainSpeed > 0)) 
            return;
        
        switch (_trainData.CurrentTrainSpeed)
        {
            case < 20:
                SetDensityAndTurbulence(_minimumDebris);
                break;
            case > 20 and < 100:
                SetDensityAndTurbulence(_normalDebris);
                break;
            case > 100:
                SetDensityAndTurbulence(_higherDebris);
                break;
        }
    }

    private void SetDensityAndTurbulence(float value)
    {
        _debrisEffect.SetFloat("Density", value);
        _debrisEffect.SetFloat("Force", value);
    }
}
