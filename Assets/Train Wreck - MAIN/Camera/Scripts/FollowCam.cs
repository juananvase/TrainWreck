using GameEvents;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class FollowCam : TrainHealthStages
{
    [Header("Components")]
    [field: SerializeField, InlineEditor] private CameraDataSO CameraData;
    [field: SerializeField] private CinemachineFollow Cinemachine { get; set; }
    [SerializeField] private BoolEventAsset OnBeginGameplay;

    private bool canStartGameplay = false;

    private void OnValidate()
    {
        Cinemachine = GetComponent<CinemachineFollow>();
    }
    private void Start()
    {
        Cinemachine.FollowOffset = CameraData.LostPosition;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnBeginGameplay.AddListener(SetStartGameplay);
    }
    protected override void OnDisable()
    {
        base.OnEnable();
        OnBeginGameplay.RemoveListener(SetStartGameplay);
    }
    private void SetStartGameplay(bool _)
    {
        canStartGameplay = true;
        trainHealth = CameraData.MiddlePosition.y;
    }
    
    private void Update()
    {
        if (canStartGameplay)
        {
            CheckTrainStage();
        }
    }

    private void CheckTrainStage()
    {
        trainHealthStage = SetTrainStage(trainHealth);
        switch (trainHealthStage)
        {
            case TrainStages.HEALTHY:
                SetCamPos(CameraData.AheadPosition);
                break;
            case TrainStages.MEDIUM:
                SetCamPos(CameraData.MiddlePosition);
                break;
            case TrainStages.LOW:
                SetCamPos(CameraData.BehindPosition);
                break;
            case TrainStages.WRECKED:
                SetCamPos(CameraData.LostPosition, CameraData.LostCamPosChangeTime);
                break;
            default:
                SetCamPos(CameraData.AheadPosition);
                break;
        }
    }

    private void SetCamPos(Vector3 newPosition)
    {
        if (Cinemachine == null)
        {
            return;
        }
        
        var smoothPos = Vector3.Lerp(Cinemachine.FollowOffset, newPosition, Time.deltaTime * CameraData.CamPosChangeTime);
        Cinemachine.FollowOffset = smoothPos;
    }
    
    private void SetCamPos(Vector3 newPosition, float camPosChangeTime)
    {
        if (Cinemachine == null)
        {
            return;
        }
        
        var smoothPos = Vector3.Lerp(Cinemachine.FollowOffset, newPosition, Time.deltaTime * camPosChangeTime);
        Cinemachine.FollowOffset = smoothPos;
    }
}