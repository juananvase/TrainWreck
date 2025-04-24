using UnityEngine;
using UnityEngine.InputSystem;

public class FaceCameraUI : MonoBehaviour
{
    [SerializeField] bool _targetAllRotations = true;
    void Update()
    {
        if (_targetAllRotations)
        {
            TargetAllRotations();
        }
        else
        {
            TargetOnlyX();
        }
    }

    private void TargetAllRotations() 
    {
        transform.rotation = Camera.main.transform.rotation;
    }
    
    private void TargetOnlyX() 
    {
        transform.eulerAngles = new Vector3(
            Camera.main.transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );
    }
}
