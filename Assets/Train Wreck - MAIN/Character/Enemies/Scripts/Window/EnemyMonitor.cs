using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using GameEvents;

[System.Serializable]
public class EnemyMonitor : MonoBehaviour
{
    [field: SerializeField] public List<EnemyAttackPoint> EnemyAttackPoints { get; private set; } = new List<EnemyAttackPoint>();
    public Dictionary<EnemyController, EnemyAttackPoint> EnemyPointAssignments { get; private set; } = new Dictionary<EnemyController, EnemyAttackPoint>();
    List<EnemyAttackPoint> availablePoints = new List<EnemyAttackPoint>(); //Create a new list to store available points

    [Header("ListeningEvents")]
    [SerializeField] private GameObjectEventAsset OnBreakObject;
    [SerializeField] private GameObjectEventAsset OnRepairObject;
    private EnemyController assignedEnemy = null;

    private void OnEnable()
    {
        OnBreakObject.AddListener(HandleBrokenWindow);
        OnRepairObject.AddListener(HandleRepairWindow);
    }

    private void OnDisable()
    {
        OnBreakObject.RemoveListener(HandleBrokenWindow);
        OnRepairObject.RemoveListener(HandleRepairWindow);
    }

    private void Start()
    {
        FindEnemyAttackPoints();
    }

    private void FindEnemyAttackPoints()
    {
        var attackPoints = GetComponentsInChildren<EnemyAttackPoint>();
        if (attackPoints.Length == 0) return;

        foreach (EnemyAttackPoint point in attackPoints)
        {
            if (point != null)
            {
                EnemyAttackPoints.Add(point);
            }
        }
    }

    public bool HasAvailablePoints()
    {
        foreach (var point in EnemyAttackPoints)
        {
            if (!point.IsOccupied) return true; //If any point is not occupied, return true
        }
        return false; //If all points are occupied, return false
    }

    public EnemyAttackPoint GetRandomAvailablePoint()
    {
        List<EnemyAttackPoint> availableAttackPoints = new List<EnemyAttackPoint>(); //Create a new list to store available points
        foreach (var point in EnemyAttackPoints)
        {
            if (!point.IsOccupied) availableAttackPoints.Add(point); //Add point to available list if it's not occupied
        }
        //If there are available points, return one randomly
        if (availableAttackPoints.Count > 0) 
        {
            return availableAttackPoints[Random.Range(0, availableAttackPoints.Count)];
        }
        return null;
    }

    public void AssignEnemy(EnemyController enemy, EnemyAttackPoint point)
    {
        //Only assign the enemy if it hasn't been assigned to this monitor point yet
        if (!EnemyPointAssignments.ContainsKey(enemy))
        {
            EnemyPointAssignments.Add(enemy, point);
            point.IsOccupied = true; // REMEMBER to mark the point as occupied
        }
    }

    public void RemoveEnemyFromPoint(EnemyAttackPoint point)
    {
        foreach (var pair in EnemyPointAssignments)
        {
            if (pair.Value == point)
            {
                var enemy = pair.Key; //Get the enemy assigned to this point
                EnemyPointAssignments.Remove(enemy);
                point.IsOccupied = false; //Free up the spot
                break; //Break cycle to avoid continuing checking the dictionary
            }
        }
    }

    public bool ContainsPoint(EnemyAttackPoint point)
    {
        foreach (EnemyAttackPoint p in EnemyAttackPoints)
        {
            if (p == point) return true;
        }
        return false;
    }

    public EnemyController GetAssignedEnemy()
    {
        if (EnemyPointAssignments.Count > 0)
        {
            //Randomly select an enemy from the enemyWindowAssignments dictionary
            int randomIndex = Random.Range(0, EnemyPointAssignments.Count);
            int currentIndex = 0;

            //Iterate over the dictionary and return the enemy at the random index
            foreach (var assignment in EnemyPointAssignments)
            {
                if (currentIndex == randomIndex)
                {
                    return assignment.Key;
                }
                currentIndex++; //Increment the index to check other windows
            }
        }
        return null;
    }

    private void HandleBrokenWindow(GameObject gameObject)
    {
        
        if (this.name != gameObject.name) return;
        if (gameObject.TryGetComponent(out EnemyMonitor monitor))
        {
            List<EnemyAttackPoint> occupiedAttackPoints = new List<EnemyAttackPoint>();
            if (monitor != null)
            {
                foreach (var entry in EnemyPointAssignments)
                {
                    if (entry.Value != null && entry.Key != null && entry.Value.IsOccupied)
                    {
                        occupiedAttackPoints.Add(entry.Value);
                    }
                }
            }
            if (occupiedAttackPoints.Count == 0) return; //If no occupied points, exit

            //Pick a random monitor point that is occupied by an enemy
            EnemyAttackPoint randomPoint = occupiedAttackPoints[Random.Range(0, occupiedAttackPoints.Count)];
            assignedEnemy = null;

            //Find the assigned enemy for the randomly chosen point
            foreach (var entry in EnemyPointAssignments)
            {
                if (entry.Value == randomPoint)
                {
                    assignedEnemy = entry.Key; //Get the enemy at the chosen point
                    break;
                }
            }
            if (assignedEnemy != null) assignedEnemy.ChangeState(assignedEnemy.SwitchAttackerState(true));
        }
        else
        {
            return;
        } 
    }

    private void HandleRepairWindow(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out EnemyMonitor window))
        {
            if (assignedEnemy != null)
            {
                assignedEnemy.ChangeState(assignedEnemy.SwitchAttackerState(false));
            }
        }
    }

    //Checks if the given breakable object is part of the monitor
    public bool ContainsObject(BreakableObject breakableObject)
    {
        return breakableObject.gameObject == this.gameObject;
    }
}