using System;
using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    
    private void OnValidate() => Rigidbody = GetComponent<Rigidbody>();
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IKnockbackable obj))
        {
            obj.GetKnockedBack(transform);
            if (Rigidbody.isKinematic == false)
            {
                Rigidbody.linearVelocity *= 0.5f;
                Rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
