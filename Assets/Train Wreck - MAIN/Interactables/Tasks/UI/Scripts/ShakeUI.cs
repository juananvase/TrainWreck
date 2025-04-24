using UnityEngine;
using PrimeTween;

public class ShakeUI : MonoBehaviour
{
    [SerializeField] private ShakeSettings _shakeSettings;

    public Tween ShakeObject(GameObject gameObject) 
    {
        return Tween.ShakeLocalPosition(gameObject.transform, _shakeSettings);
    }

}
