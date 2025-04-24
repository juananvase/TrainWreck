using GameEvents;
using UnityEngine;

public abstract class TrainHealthStages : MonoBehaviour
{
    [SerializeField] private Vector2EventAsset OnTrainTrainHealthUpdated;
    protected float trainHealth;
    protected TrainStages trainHealthStage;    
    
    
    protected virtual void OnEnable()
    {
        OnTrainTrainHealthUpdated.AddListener(OnTrainHealthUpdated);
    }

    protected virtual void OnDisable()
    {
        OnTrainTrainHealthUpdated.RemoveListener(OnTrainHealthUpdated);
    }

    protected virtual void OnTrainHealthUpdated(Vector2 health)
    {
        trainHealth = health.x;
    }
    
    protected TrainStages SetTrainStage(float health)
    {
        switch (health)
        {
            case <= 20f and > 1f:
                return TrainStages.LOW;
            case >= 100f:
                return TrainStages.HEALTHY;
            case <= 100f and >= 20f:
                return TrainStages.MEDIUM;
            case < 1f:
                return TrainStages.WRECKED;
            default:
                return TrainStages.MEDIUM;
        }
    }
}
