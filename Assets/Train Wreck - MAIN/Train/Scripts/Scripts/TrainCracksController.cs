using UnityEngine;
using GameEvents;
using System.Collections;

public class TrainCracksController : MonoBehaviour
{
    [SerializeField] private Vector2EventAsset OnTrainTrainHealthUpdated;
    [SerializeField] private Renderer MeshRenderer;
    private float currentFill;
    public AnimationCurve fillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float animationDuration = 1.0f;
    private Coroutine curveCoroutine;

    private void OnEnable()
    {
        OnTrainTrainHealthUpdated.AddListener(UpdateTexture);
       
    }

    private void OnDisable()
    {
        OnTrainTrainHealthUpdated.RemoveListener(UpdateTexture);
    }

    private void Start()
    {
        MeshRenderer =GetComponent<MeshRenderer>();
        MeshRenderer.material.SetFloat("_Fill", 1);
    }

    public void UpdateTexture(Vector2 health)
    {
        float targetFill = health.x / health.y;

        if (curveCoroutine != null)
        {
            StopCoroutine(curveCoroutine);
        }

        curveCoroutine = StartCoroutine(AnimateFill(Mathf.Abs(targetFill)));
    }

    private IEnumerator AnimateFill(float targetFill)
    {
        float startFill = MeshRenderer.material.GetFloat("_Fill");
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            float curveValue = fillCurve.Evaluate(t);
            float currentFill = Mathf.Lerp(startFill, targetFill, curveValue);

            MeshRenderer.material.SetFloat("_Fill", currentFill);
            yield return null;
        }

        // Ensure the final value is set accurately
        MeshRenderer.material.SetFloat("_Fill", targetFill);
    }
}


