using System.Collections;
using EditorTools;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseProgressBar : MonoBehaviour
{
    [SerializeField] protected Image _fillBar;
    [SerializeField] protected float _lerpDuration = 0.25f;

    protected IEnumerator _lerpCoroutine;

    protected IEnumerator LerpBar(float targetFill, float duration, Image bar)
    {
        float currentFill = bar.fillAmount;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            float fill = Mathf.Lerp(currentFill, targetFill, progress);
            bar.fillAmount = fill;

            yield return null;
        }
    }
}
