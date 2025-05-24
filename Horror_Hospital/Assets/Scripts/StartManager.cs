using UnityEngine;
using TMPro;
using System.Collections;

public class StartManager : MonoBehaviour
{
    [Header("🖼 UI 구성")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI guideText;
    public GameObject crossHairCanvas;

    [Header("🔊 오디오")]
    public AudioSource bgm;

    [Header("🎮 플레이어")]
    public RigidbodyFPSController playerController;

    [Header("🎞 효과")]
    public TypewriterEffect typewriterEffect;

    [Header("⚙ 설정")]
    public float titleFadeInDuration = 1f;
    public float titleHoldDuration = 1.5f;
    public float titleFadeOutDuration = 0.5f;
    public float guideHoldDuration = 2f;

    private void Start()
    {
        guideText.gameObject.SetActive(false);
        crossHairCanvas.SetActive(false);
        playerController.canControl = false;
        bgm.Play();

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        titleText.text = "Horror\nHospital";
        yield return FadeIn(canvasGroup, titleFadeInDuration);
        yield return new WaitForSeconds(titleHoldDuration);
        yield return FadeOut(canvasGroup, titleFadeOutDuration);
        titleText.gameObject.SetActive(false);

        guideText.gameObject.SetActive(true);
        yield return FadeIn(canvasGroup, titleFadeInDuration);
        yield return typewriterEffect.PlayTyping(guideText,
            "Enter the operation room.\n\n" +
            "Once the door shuts behind you, stay alert.\n\n" +
            "If everything feels normal, press the green button and leave.\n\n" +
            "But if something feels... off — hit the red button and get out. Fast.", 
            canvasGroup
            );
        yield return new WaitForSeconds(guideHoldDuration);
        yield return FadeOut(canvasGroup, titleFadeOutDuration);
        guideText.gameObject.SetActive(false);

        canvasGroup.gameObject.SetActive(false);
        yield return FadeOutAudio(bgm, 1f);

        playerController.canControl = true;
        crossHairCanvas.SetActive(true);
        GameManager.Instance.StartGameFlow();
    }

    private IEnumerator FadeIn(CanvasGroup cg, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(0, 1, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;
    }

    private IEnumerator FadeOut(CanvasGroup cg, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(1, 0, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0;
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float t = 0f;
        while (t < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}