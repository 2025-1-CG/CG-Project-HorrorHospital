using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환해도 유지
    }

    public void Play()
    {
        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void Stop()
    {
        bgmSource.Stop();
    }

    public void SetVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void FadeTo(float targetVolume, float duration)
    {
        StartCoroutine(FadeVolumeCoroutine(targetVolume, duration));
    }

    private IEnumerator FadeVolumeCoroutine(float targetVolume, float duration)
    {
        float start = bgmSource.volume;
        float t = 0f;

        while (t < duration)
        {
            bgmSource.volume = Mathf.Lerp(start, targetVolume, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }
}