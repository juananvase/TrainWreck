using FMODUnity;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private EventReference GreetingEvent;
    private Rigidbody rb;
    private Animator animator;
    float rotateSpeed = 200.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        transform.rotation = new Quaternion(0, 0, 0, 0);
        
        if (!GreetingEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(GreetingEvent, transform.position);
        }
        
    }

    public void RotateCharacter(float input)
    {
        Quaternion currentRotation = transform.rotation;
        if (input > 0.1f)
        {
            Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y - rotateSpeed * Time.deltaTime, currentRotation.eulerAngles.z);
            rb.MoveRotation(targetRotation);
        }
        else if (input < -0.1f)
        {
            Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y + rotateSpeed * Time.deltaTime, currentRotation.eulerAngles.z);
            rb.MoveRotation(targetRotation);
        }
    }

    public void Wave(bool input) 
    {
        animator.SetTrigger("Wave");
        PlayGreetingSFX();
    }

    public void Celebrate(bool input) 
    {
        animator.SetTrigger("Jump");
        PlayGreetingSFX();
    }

    public void ThumbsUp(bool input)
    {
        animator.SetTrigger("ThumbsUp");
        PlayGreetingSFX();
    }
    
    //audio 
    private void PlayGreetingSFX()
    {
        RuntimeManager.PlayOneShot(GreetingEvent, transform.position);
    }
    
}

