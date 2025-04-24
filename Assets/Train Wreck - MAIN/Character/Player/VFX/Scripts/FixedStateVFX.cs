using UnityEngine;
using UnityEngine.VFX;

public class FixedStateVFX : MonoBehaviour
{
    [SerializeField] private VisualEffect _sparklesVFX;
    
    private void Awake()
    {
        _sparklesVFX = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        _sparklesVFX.SetVector3("TargetPos", transform.position);
    }
}
