using System;
using System.Collections;
using GameEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EmptyFuelMessage : MonoBehaviour
{
    [SerializeField] private FireboxTaskDataSO _fireboxData;
    public UnityEvent WriteMessage;
    public UnityEvent CleanMessage;

    private void Start()
    {
        StartCoroutine(CheckNotMoving());
    }
    private IEnumerator CheckNotMoving() 
    {
        while (true) 
        {
            if (_fireboxData.CurrentFuelLevel <= _fireboxData.LowThreshold)
            {
                WriteMessage.Invoke();
                yield return new WaitForSeconds(3f);
                CleanMessage.Invoke();
                yield return new WaitForSeconds(5f);
            }

            yield return null;
        }
    }
}
