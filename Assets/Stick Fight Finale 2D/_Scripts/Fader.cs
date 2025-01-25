using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour
{
    public Image fadeOverlay;
    public float fadeDuration = 3f;

    void Start()
    {
        // Ensure the overlay is fully transparent at the start
        if (fadeOverlay != null)
        {
            fadeOverlay.color = new Color(0, 0, 0, 0);
        }
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }
    }
}
