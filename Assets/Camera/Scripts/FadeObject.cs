using UnityEngine;

public class FadeObject : MonoBehaviour
{
    private float fadeSpeed = 5f;

    private float targetFade = 0f; // 1 is fully visible, 2 is invisible (will fix)
    private float currentFade = 0f;

    private Renderer[] objectRenderers;
    private MaterialPropertyBlock fadePropertyBlock;

    void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>();
        fadePropertyBlock = new MaterialPropertyBlock();
    }

    public void FadeOut() => targetFade = 2f;
    public void FadeIn() => targetFade = 1f;
    
    void Update()
    {
        currentFade = Mathf.MoveTowards(currentFade, targetFade, Time.deltaTime * fadeSpeed);

        fadePropertyBlock.SetFloat("_FadeAmount", currentFade);

        foreach (var renderer in objectRenderers)
        {
            renderer.SetPropertyBlock(fadePropertyBlock);
        }
    }
    
    
}