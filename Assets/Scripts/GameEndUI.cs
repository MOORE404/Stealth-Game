using System.Collections;
using TMPro;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{

    public TextMeshProUGUI tmpText;
    private float fadeDuration = 3f;
    private float startFontSize = 24f;
    private float endFontSize = 35f;

    void Awake()
    {
        StartCoroutine(FadeInTextWithSize());
    }

    IEnumerator FadeInTextWithSize()
    {
        tmpText.ForceMeshUpdate();

        Color originalColor = tmpText.color;
        originalColor.a = 0;
        tmpText.color = originalColor;

        tmpText.fontSize = startFontSize;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            
            float alpha = Mathf.Lerp(0f, 1f, t);
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            
            tmpText.fontSize = Mathf.Lerp(startFontSize, endFontSize, t);

            yield return null;
        }

        
        tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        tmpText.fontSize = endFontSize;
    }

    
    public void ResetAndFadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInTextWithSize());
    }
}
