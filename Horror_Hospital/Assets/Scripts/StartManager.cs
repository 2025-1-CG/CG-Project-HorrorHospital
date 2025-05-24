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
    public AudioSource typingAudioSource;          // 👈 추가
    public AudioClip typingSound;

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
            yield return TypeText("Enter the operation room.\n\n" +
"Once the door shuts behind you, stay alert.\n\n" +
"If everything feels normal, press the green button and leave.\n\n" +
"But if something feels... off — hit the red button and get out. Fast.");
            yield return new WaitForSeconds(guideHoldDuration);
            yield return FadeOut(canvasGroup, titleFadeOutDuration);
            guideText.gameObject.SetActive(false);
        }

        // 3. UI 종료 + 조작/크로스헤어 활성화
        canvasGroup.gameObject.SetActive(false);
        if (bgm != null)
            yield return StartCoroutine(FadeOutAudio(bgm, 1f));
        if (playerController != null) playerController.canControl = true;
        if (crossHairCanvas != null) crossHairCanvas.SetActive(true);

        // 4. 게임 시작
        GameManager.Instance.StartGameFlow();
    }

    private IEnumerator TypeText(string fullText)
    {
        guideText.text = "";

        // 🎧 타이핑 사운드 루프 재생 시작
        if (typingAudioSource != null && typingSound != null)
        {
            typingAudioSource.clip = typingSound;
            typingAudioSource.loop = true;
            typingAudioSource.Play();
        }

        foreach (char c in fullText)
        {
            guideText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // 🎧 타이핑 사운드 정지
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
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

    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // 다음 재생을 위해 원복
    }
}