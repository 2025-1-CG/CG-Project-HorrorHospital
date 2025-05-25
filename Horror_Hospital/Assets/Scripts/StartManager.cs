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

        // BGM 시작 (처음은 크게)
        BGMManager.Instance.SetVolume(1.0f);
        BGMManager.Instance.Play();

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        titleText.text = "Horror\nHospital";
        yield return FadeIn(canvasGroup, titleFadeInDuration);
        yield return new WaitForSeconds(titleHoldDuration);
        yield return FadeOut(canvasGroup, titleFadeOutDuration);
        titleText.gameObject.SetActive(false);

        // 타이핑 준비
        guideText.text = "";
        guideText.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        yield return FadeIn(canvasGroup, titleFadeInDuration);

        // 타이핑 효과 실행
        yield return typewriterEffect.PlayTyping(guideText,
            "Enter the operation room.\n\n" +
            "Once the door shuts behind you, stay alert.\n\n" +
            "If everything feels normal, press the green button and leave.\n\n" +
            "But if something feels... off — hit the red button and get out. Fast.");

        yield return new WaitForSeconds(guideHoldDuration);
        yield return FadeOut(canvasGroup, titleFadeOutDuration);

        guideText.gameObject.SetActive(false);
        canvasGroup.gameObject.SetActive(false);

        // BGM 줄이기 (잔잔하게 유지)
        BGMManager.Instance.FadeTo(0.1f, 1.2f);

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
}