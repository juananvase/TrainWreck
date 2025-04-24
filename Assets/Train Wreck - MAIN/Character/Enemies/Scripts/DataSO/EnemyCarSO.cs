using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCarSO", menuName = "Scriptable Objects/EnemyCarSO")]
public class EnemyCarSO : ScriptableObject
{
    [Header("Driving Animation Settings")]
    public Vector2 DrivingFrequency;
    [field: Tooltip("How far the enemy moves up and down when driving. Higher limit in random range"), SerializeField]
    public Vector2 DrivingMagnitude;
    [field: Tooltip("How far the enemy moves in random directions when attacking. Higher limit in random range"), SerializeField]
    public Vector2 InfiniteMagnitude;
    [field: Tooltip("How long the enemy shakes when it gets hit."), SerializeField]
    public float ShakeDuration = 5f;
    [field: Tooltip("How long the enemy shakes when it gets hit."), SerializeField]
    public float PerlinSpeed = 5f;
    [field: Tooltip("How long the enemy shakes when it gets hit."), SerializeField]
    public Vector2 PerlinOffset;

    [field: Tooltip("How much noise the enemy has in its behaviour"), SerializeField, FoldoutGroup("Enemy Behaviour")]
    public Vector2 EnemyToStopTrain { get; set; }

    [field: SerializeField, FoldoutGroup("Enemy Behaviour")]
    public Vector2 EnemyToSlowTrainSpeed { get; set; }

    [field: SerializeField, FoldoutGroup("Enemy Behaviour")]
    public Vector2 EnemyToNormalTrainSpeed { get; set; }

    [field: SerializeField, FoldoutGroup("Enemy Behaviour")]
    public Vector2 EnemyToFastTrainSpeed { get; set; }
}