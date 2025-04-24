using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameManagerDataSO", menuName = "Scriptable Objects/GameManagerDataSO")]
public class GameManagerDataSO : ScriptableObject
{
    [field: SerializeField, FoldoutGroup("PlayerInput")] public PlayerInputEventAsset OnReceivingPlayerInputComponent { get; private set; }
    [field: SerializeField, FoldoutGroup("Tutoralization Panel")] public ImageEventAsset OnReceivingImageComponent { get; private set; }
}
