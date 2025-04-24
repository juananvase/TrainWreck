using System.Collections;
using GameEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LoseFunctionality : BaseMenusFunctionality
{
    [SerializeField] private Vector2EventAsset _onTrainHealthUpdated;
    [SerializeField] private BoolEventAsset _onGameLost;
    [SerializeField] private float _gameLostDelay;
    public UnityEvent OnGameLost;

    protected override void OnEnable()
    {
        base.OnEnable();
        _onTrainHealthUpdated.AddListener(OnHealthUpdated);
    }


    private void OnDisable()
    {
        _onTrainHealthUpdated.RemoveListener(OnHealthUpdated);
    }

    private void OnHealthUpdated(Vector2 arg0)
    {
        if (arg0.x <= 0) 
        {
            StartCoroutine(AfterLostDelay());
            return;
        }
    }

    private IEnumerator AfterLostDelay() 
    {
        yield return new WaitForSeconds(1f);
        _onGameLost.Invoke(true);
        yield return new WaitForSeconds(_gameLostDelay);
        OnGameLost.Invoke();
    }

}
