using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    [field: Tooltip("Determine the max health of the enemy. Increasing this value will make the enemy harder to defeat, requiring more damage to be dealt to destroy it."), SerializeField]
    public float Health { get; private set; } = 100f;

    [field: Tooltip("Delay between each time the enemy shoots at a target. Increasing this value will make the enemy shoot less frequently"), SerializeField]
    public float ShootDelay { get; private set; } = 5f;

    [field: Tooltip("Delay before the enemy begins shooting after arriving at their position. Increasing this value will cause the enemy to wait longer before starting to shoot"), SerializeField]
    public float InitialShootDelay { get; private set; } = 5f;

    [field: Tooltip("Amount of time it takes between the Shooter and Bomber to switch between each other. Increasing this value will make the switch occur more slowly"), SerializeField]
    public float SwitchingDuration { get; private set; } = 2f;

    [field: Tooltip("Duration of time waited to spawn enemies when the level starts"), SerializeField]
    public float InitialDelay { get; set; }

    [field: Tooltip("Duration of time waited to spawn each enemy"), SerializeField]
    public float EnemySpawnDelay { get; set; }

    [field: Tooltip("The speed of the enemy based on how fast the train is going"), SerializeField, FoldoutGroup("Enemy Speeds")]
    public float EnemyToWreckedTrainSpeed { get; set; } = 2f;

    [field: SerializeField, FoldoutGroup("Enemy Speeds")]
    public float EnemyToSlowTrainSpeed { get; set; } = 2f;

    [field: SerializeField, FoldoutGroup("Enemy Speeds")]
    public float EnemyToNormalTrainSpeed { get; set; } = 2f;

    [field: SerializeField, FoldoutGroup("Enemy Speeds")]
    public float EnemyToFastTrainSpeed { get; set; } = 2f;

    [field: Tooltip("Delay between each time the enemy throws bombs inside the car"), SerializeField]
    public float BombThrowDelay { get; private set; } = 5f;
    
    [field: Tooltip("Whether or not the enemy can spawn in the level."), SerializeField]
    public bool CanSpawnEnemies { get; set; }
}