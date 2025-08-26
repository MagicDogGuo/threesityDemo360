using UnityEngine;
using System.Collections;
public class VRCameraFade : MonoBehaviour
{
    public GameObject fadeQuad; // Assign the FadeQuad GameObject here
    private Material fadeMaterial; // Material with the custom shader
    private Coroutine currentFadeCoroutine;

    protected void Awake()
    {
        if (fadeQuad != null)
        {
            fadeQuad.SetActive(false);
            // Get the material from the FadeQuad's Renderer
            fadeMaterial = fadeQuad.GetComponent<Renderer>().material;
        }
    }

    public void FadeIn(float duration)
    {
        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
        fadeQuad.SetActive(true);
        SetFadeAmount(1); // Set the fade amount to 1 (black)
        currentFadeCoroutine = StartCoroutine(Fade(1, 0, duration)); // Fade from black to clear
    }

    public void FadeOut(float duration)
    {
        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
        fadeQuad.SetActive(true);
        SetFadeAmount(0); // Set the fade amount to 0 (clear)
        currentFadeCoroutine = StartCoroutine(Fade(0, 1, duration)); // Fade from clear to black
    }

    private IEnumerator Fade(float startAmount, float endAmount, float duration)
    {
        float elapsed = 0f;
        duration = Mathf.Max(duration, 0.1f); // Ensure the duration is at least 0.1 seconds
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(startAmount, endAmount, elapsed / duration);
            SetFadeAmount(fadeAmount);
            yield return null;
        }

        SetFadeAmount(endAmount);
        fadeQuad.SetActive(false);
    }

    private void SetFadeAmount(float amount)
    {
        if (fadeMaterial != null)
        {
            fadeMaterial.SetFloat("_FadeAmount", amount); // Update the shader property
        }
    }
}
