using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // 검정 이미지
    public float fadeDuration = 1f;

    public IEnumerator FadeOut()
    {
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }


    public IEnumerator FadeIn()
    {
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 확실하게 0으로 고정
        color.a = 0f;
        fadeImage.color = color;
    }

}
