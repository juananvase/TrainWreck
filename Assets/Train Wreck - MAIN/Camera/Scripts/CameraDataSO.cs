using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraData", menuName = "Scriptable Objects/Camera")]
public class CameraDataSO : ScriptableObject
{
    [field: Header("Camera Position")]
    [field: SerializeField] public Vector3 AheadPosition { get; private set;}
    [field: SerializeField] public Vector3 MiddlePosition { get; private set;}
    [field: SerializeField] public Vector3 BehindPosition { get; private set;}
    [field: SerializeField] public Vector3 LostPosition { get; private set;}
    [field: Tooltip("The transition time to change camera position"), SerializeField]
    public float CamPosChangeTime { get; private set; } = 2f;
    [field: SerializeField] public float LostCamPosChangeTime { get; private set; } = 0.5f;
    
    [field: Header("Current Train Stage")]
    [field: Tooltip("The speed states of the train: <br> WRECKED(No Speed), SLOW, NORMAL, FAST, OVERCLOCKED"), SerializeField]
    public string CurrentTrainStage { get; set;}

    [field: Header("Camera Shake Sources")]
    [field: SerializeField, FoldoutGroup("Mounted Gun Recoil")] public CinemachineImpulseDefinition ImpulseSourceRecoil { get; private set; }
    [field: SerializeField, FoldoutGroup("Bomb Explosion")] public CinemachineImpulseDefinition ImpulseSourceBombExplosion { get; private set; }
    [field: SerializeField, FoldoutGroup("Furnace Explosion")] public CinemachineImpulseDefinition ImpulseSourceFurnaceExplosion { get; private set; }
}
