using System;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class GameTutoralization : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Data")] private bool _log = false;
    [SerializeField, FoldoutGroup("Events")] private GameObjectEventAsset _onRepairObject;
    [SerializeField, FoldoutGroup("Events")] private FloatEventAsset _onFuelLevelUpdated;

    private bool _windowFIxed = false;
    private bool _pipeFixed = false;
    private bool _fireboxLoaded = false;

    public UnityEvent OnFixedFirstWindow;
    public UnityEvent OnFixedFirstPipe;
    public UnityEvent OnLoadFireboxFirstTime;


    private void OnEnable()
    {
        if (GameManager.Instance.ShowTutorial) 
            gameObject.SetActive(true);
        else 
            gameObject.SetActive(false);

        _onRepairObject.AddListener(CheckRepairObjectTaskComplete);
        _onFuelLevelUpdated.AddListener(CheckLoadFireboxFirstTime);
    }
    private void OnDisable()
    {
        _onRepairObject.RemoveListener(CheckRepairObjectTaskComplete);
        _onFuelLevelUpdated.AddListener(CheckLoadFireboxFirstTime);
    }

    private void CheckLoadFireboxFirstTime(float value)
    {
        if (value > 0 && !_fireboxLoaded)
        {
            _fireboxLoaded = true;
            OnLoadFireboxFirstTime?.Invoke();
        }
    }

    private void CheckRepairObjectTaskComplete(GameObject taskObject)
    {
        //Check if the interactable gameobject is a pipe or window
        BreakableObject breakableObject = taskObject.GetComponent<BreakableObject>();
        BreakableWindow breakableWindow = breakableObject as BreakableWindow;
        BreakablePipe breakablePipe = breakableObject as BreakablePipe;

        if (breakableWindow != null && !_windowFIxed) 
        {
            _windowFIxed = true;
            OnFixedFirstWindow?.Invoke();

            if (_log) 
                Debug.Log("First Window Fixed");

            return;
        }

        if (breakablePipe != null && !_pipeFixed)
        {
            _pipeFixed = true;
            OnFixedFirstPipe?.Invoke();

            if (_log)
                Debug.Log("First Pipe Fixed");

            return;
        }
    }
}
