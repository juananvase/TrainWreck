using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using Sirenix.OdinInspector;
using EditorTools;

public class FootprintsVFX : PooledObject
{
    [Tooltip("How long the decal will stay, before it dissapears")]
    public float lifespan = 10.0f;
    [SerializeField] private DecalProjector opacity;
    [BoxGroup("Debug"), ShowInInspector] public float timeLeft;
    private Vector3 size;
    private float elapsedTime;

    public void Start()
    {
        opacity = GetComponent<DecalProjector>();
        elapsedTime = 0;
        StartCoroutine(DissapearingFootprintRoutine());
    }

    public IEnumerator DissapearingFootprintRoutine()
    {
        while (elapsedTime < lifespan)
        {
            elapsedTime += Time.deltaTime;
            timeLeft = (lifespan - elapsedTime) / lifespan;
            this.transform.localScale = new Vector3(size.x * timeLeft, size.y * timeLeft, size.z * timeLeft);
            opacity.material.color = new Color(0, 0, 0, timeLeft);
            yield return null;
        }
        ReturnToPool();

        yield return null;
    }

}
