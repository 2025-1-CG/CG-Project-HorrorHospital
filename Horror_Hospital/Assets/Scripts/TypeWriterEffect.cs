using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float typeSpeed = 0.05f;
    public AudioSource typingAudioSource;
    public AudioClip typingSound;

    public IEnumerator PlayTyping(TextMeshProUGUI text, string message, CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        // Fade In
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / 1f);
            yield return null;
        }

        // 시작 타이핑
        text.text = "";

        // 사운드 시작
        if (typingAudioSource != null && typingSound != null)
        {
            typingAudioSource.clip = typingSound;
            typingAudioSource.loop = true;
            typingAudioSource.Play();
        }

        foreach (char c in message)
        {
            text.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // 사운드 정지
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }

        yield return new WaitForSeconds(2f); // 유지 시간

        // Fade Out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 1f);
            yield return null;
        }

        canvasGroup.gameObject.SetActive(false);
    }
}