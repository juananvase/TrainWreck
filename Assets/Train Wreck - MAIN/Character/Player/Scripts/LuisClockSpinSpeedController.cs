using UnityEngine;

public class LuisClockSpinSpeedController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetFloat("ClockSpeed", _speed);
    }
}
