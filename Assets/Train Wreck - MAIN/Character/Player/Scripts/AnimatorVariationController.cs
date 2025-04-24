using System.Collections;
using FMODUnity;
using UnityEngine;

public class AnimatorVariationController : MonoBehaviour
{
    private Animator _animator;

    [Header("Animation Variators")]
    [field: SerializeField] private float _initialDelay;
    [field: SerializeField] private float _animationSpeedMultiplier;
    [field: SerializeField] private string _animationName;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(TriggerAnimations());
    }

    IEnumerator TriggerAnimations() 
    {
        yield return new WaitForSeconds(_initialDelay);
        _animator.Play(_animationName);
        _animator.SetFloat("AnimationSpeed", _animationSpeedMultiplier);
    }

}
