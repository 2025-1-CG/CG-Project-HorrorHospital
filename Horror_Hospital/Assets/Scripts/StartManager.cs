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

    [Header("⚙ 설정")]
    public float titleFadeInDuration = 1f;
    public float titleHoldDuration = 1.5f;
    public float titleFadeOutDuration = 0.5f;
    public float guideHoldDuration = 2f;
    public float typeSpeed = 0.05f;

    private void Start()
    {
        // 초기 상태 설정
        if (guideText != null) guideText.gameObject.SetActive(false);
        if (crossHairCanvas != null) crossHairCanvas.SetActive(false);
        if (playerController != null) playerController.canControl = false;
        if (bgm != null) bgm.Play();

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // 1. 타이틀 표시
        if (titleText != null)
        {
            titleText.text = "Horror\nHospital";
            yield return FadeIn(canvasGroup, titleFadeInDuration);
            yield return new WaitForSeconds(titleHoldDuration);
            yield return FadeOut(canvasGroup, titleFadeOutDuration);
            titleText.gameObject.SetActive(false);
        }

        // 2. 안내 문구 등장
        if (guideText != null)
        {
            yield return FadeIn(canvasGroup, titleFadeInDuration);
            guideText.gameObject.SetActive(true);
            yield return TypeText("Enter The Operation Room.\nand when the door closes, You have to judge the situation.\nIf all seems normal, press the Green button and leave.\nIf you sense anything strange, press the Red button and quickly exit.");
            yield return new WaitForSeconds(guideHoldDuration);
            yield return FadeOut(canvasGroup, titleFadeOutDuration);
            guideText.gameObject.SetActive(false);
        }

        // 3. UI 종료 + 조작/크로스헤어 활성화
        canvasGroup.gameObject.SetActive(false);
        if (bgm != null) bgm.Stop();
        if (playerController != null) playerController.canControl = true;
        if (crossHairCanvas != null) crossHairCanvas.SetActive(true);

        // 4. 게임 시작
        GameManager.Instance.StartGameFlow();
    }

    private IEnumerator TypeText(string fullText)
    {
        guideText.text = "";
        foreach (char c in fullText)
        {
            guideText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
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