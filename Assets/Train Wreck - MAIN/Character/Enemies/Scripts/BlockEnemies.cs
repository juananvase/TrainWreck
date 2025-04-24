using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BlockEnemies : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            //gg
        }
    }
}
