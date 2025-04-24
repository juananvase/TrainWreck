using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class FireBase : MonoBehaviour
{
    [field: SerializeField, InlineEditor] public FireDataSO FireData { get; private set; }
    [SerializeField] protected AudioSourcesSOData audioData;

    protected List<GameObject> ActiveFires => FireData.ActiveFires;


    protected virtual void Start()
    {
        FireData.ActiveFires = new List<GameObject>();
    }
    
    protected void PopulateList(GameObject fireInstance)
    {
        ActiveFires.Add(fireInstance);
    }

    protected void RemoveFromList(GameObject fireInstance)
    {
        ActiveFires.Remove(fireInstance);
    }

}