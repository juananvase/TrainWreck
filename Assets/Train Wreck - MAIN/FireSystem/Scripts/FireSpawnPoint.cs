using UnityEngine;

public class FireSpawnPoint : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Vector3 Direction => transform.forward;
    public Quaternion Rotation => transform.rotation;
}
