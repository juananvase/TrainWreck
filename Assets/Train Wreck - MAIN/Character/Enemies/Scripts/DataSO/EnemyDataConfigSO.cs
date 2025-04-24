using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy/EnemyConfigs")]
public class EnemyDataConfigSO : ScriptableObject
{
    public EnemyData[] enemyConfigs;
}
