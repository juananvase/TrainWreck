using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WaterController : MonoBehaviour
{
    [SerializeField]private WaterSurface water;
    [SerializeField]private TrainDataSO _trainData;

    [SerializeField] private float _noMovSpeed;
    [SerializeField] private float _littleMovSpeed;
    [SerializeField] private float _midMovSpeed;
    [SerializeField] private float _fastMovSpeed;


    private void Start()
    {
        water = GetComponent<WaterSurface>();
    }

    private void Update()
    {
        switch (_trainData.CurrentTrainSpeed)
        {
            case 0:
                SetWaterTurbulence(_noMovSpeed); 
                break;
            case < 20:
                SetWaterTurbulence(_littleMovSpeed);
                break;
            case > 20 and < 100:
                SetWaterTurbulence(_midMovSpeed);
                break;
            case > 100:
                SetWaterTurbulence(_fastMovSpeed);
                break;
        }
    }

    private void SetWaterTurbulence(float value)
    {
        water.ripplesWindSpeed = value;
        water.ripplesCurrentSpeedValue = value;
    }
}
