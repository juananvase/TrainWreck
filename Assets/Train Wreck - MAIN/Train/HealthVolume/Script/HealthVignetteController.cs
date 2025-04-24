using GameEvents;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class HealthVignetteController : MonoBehaviour
{
    [SerializeField] private Volume _healthVignette;
    private Vignette vignette;
    [SerializeField] private Vector2EventAsset OnTrainHealthUpdate;
    private float animationDuration;
    public AnimationCurve fillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    private void OnEnable()
    {
        OnTrainHealthUpdate.AddListener(UpdateVignetteValues);
    }

    private void OnDisable()
    {
        OnTrainHealthUpdate.RemoveListener(UpdateVignetteValues);
    }

    private void Start()
    {
        _healthVignette = GetComponent<Volume>();
    }

    private void UpdateVignetteValues(Vector2 trainHealth)
    {
        if (_healthVignette != null) 
        {
            if (_healthVignette.profile.TryGet(out vignette))
            {
                float targetFill = 1.0f - (trainHealth.x / trainHealth.y);
                StartCoroutine(AnimateVignette(targetFill));
            }
        }
    }

    private IEnumerator AnimateVignette(float targetFill)
    {
        float startFill = vignette.intensity.value;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            float curveValue = fillCurve.Evaluate(t);
            float currentFill = Mathf.Lerp(startFill, targetFill, curveValue);

            vignette.intensity.value = currentFill;
            yield return null;
        }

        // Ensure the final value is set accurately
        vignette.intensity.value = targetFill;
    }
}
