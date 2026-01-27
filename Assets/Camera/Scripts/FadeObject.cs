using UnityEngine;

public class FadeObject : MonoBehaviour
{
    private float fadeSpeed = 5f;

    private float targetFade = 0f; // 1 is fully visible, 2 is invisible (will fix)
    private float currentFade = 0f;

    private Renderer[] objectRenderers;
    private MaterialPropertyBlock fadePropertyBlock;

    private GameObject childGlassObject;
    private bool glassHidden;

    void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>();
        fadePropertyBlock = new MaterialPropertyBlock();
        
        Transform glass = transform.Find("Glass");
        if (glass != null) childGlassObject = glass.gameObject;
    }

    public void FadeOut()
    {
        targetFade = 2f;

        if (childGlassObject != null && !glassHidden)
        {
            childGlassObject.SetActive(false);
            glassHidden = true;
        }
    }

    public void FadeIn()
    {
        targetFade = 1f;
        
        if (childGlassObject != null && glassHidden)
        {
            childGlassObject.SetActive(true);
            glassHidden = false;
        }
    }

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