using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Canvas gameOverCanvas;
    public UnityEngine.UI.Image fadePanel;
    public TextMeshProUGUI gameOverText;

    [Header("Settings")]
    public float fadeDuration = 2f;
    public string successMessage = "GAME CLEAR!";
    public string failureMessage = "GAME OVER";

    [Header("Input Settings")]
    public KeyCode retryKey = KeyCode.R;
    public KeyCode exitKey = KeyCode.Escape;

    [Header("Visual Styling")]
    public Color successColor = new Color(0.2f, 0.8f, 0.2f, 1f); // 밝은 초록
    public Color failureColor = new Color(0.9f, 0.2f, 0.2f, 1f); // 밝은 빨강
    public Color instructionColor = new Color(0.8f, 0.8f, 0.8f, 1f); // 연한 회색
    public float textGlowIntensity = 2f;
    public float titlePulseSpeed = 2f;
    public float textShadowOffset = 3f;

    private bool gameOverActive = false;
    private Color originalTextColor;
    private bool isSuccess;

    void Start()
    {
        // 시작할 때는 Game Over UI를 숨김
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);

        // 원래 텍스트 색상 저장
        if (gameOverText != null)
            originalTextColor = gameOverText.color;
    }

    void Update()
    {
        // 게임오버 상태에서 키보드 입력 처리
        if (gameOverActive)
        {
            if (Input.GetKeyDown(retryKey))
            {
                RetryGame();
            }
            else if (Input.GetKeyDown(exitKey))
            {
                ExitGame();
            }
        }
    }

    public void ShowGameOver(bool success)
    {
        isSuccess = success;
        StartCoroutine(GameOverSequence(success));
    }

    void SetupTextStyling(bool success)
    {
        if (gameOverText == null) return;

        // 텍스트 크기와 정렬
        gameOverText.fontSize = success ? 72f : 64f;
        gameOverText.fontStyle = FontStyles.Bold;
        gameOverText.alignment = TextAlignmentOptions.Center;

        // 그림자 효과
        gameOverText.fontMaterial = CreateTextMaterial(success);

        // 텍스트 색상
        gameOverText.color = success ? successColor : failureColor;

        // 글로우 효과를 위한 Outline 설정
        gameOverText.outlineWidth = 0.2f;
        gameOverText.outlineColor = success ?
            new Color(successColor.r, successColor.g, successColor.b, 0.5f) :
            new Color(failureColor.r, failureColor.g, failureColor.b, 0.5f);
    }

    Material CreateTextMaterial(bool success)
    {
        Material newMat = new Material(gameOverText.fontMaterial);

        // 언더레이 설정
        newMat.SetFloat("_UnderlayOffsetX", textShadowOffset);
        newMat.SetFloat("_UnderlayOffsetY", -textShadowOffset);
        newMat.SetFloat("_UnderlayDilate", 0.5f);
        newMat.SetColor("_UnderlayColor", new Color(0, 0, 0, 0.8f));

        // 글로우 효과
        newMat.SetFloat("_GlowPower", textGlowIntensity);
        newMat.SetColor("_GlowColor", success ? successColor : failureColor);

        return newMat;
    }

    string FormatGameOverText(bool success)
    {
        // 성공 or 실패 메시지
        string mainMessage = success ? successMessage : failureMessage;
        string formattedMain = $"<size=120%><b>{mainMessage}</b></size>";

        // 재시작 or 게임 종료
        string instructions = $"\n\n<size=60%><color=#{ColorUtility.ToHtmlStringRGB(instructionColor)}>" +
                            $"<b>[R]</b> RETRY GAME\n" +
                            $"<b>[Esc]</b> EXIT GAME\n" +
                            $"</color></size>";

        return formattedMain + instructions;
    }

    IEnumerator GameOverSequence(bool success)
    {
        gameOverActive = true;

        // 게임 일시정지
        Time.timeScale = 0f;

        // Canvas 활성화
        gameOverCanvas.gameObject.SetActive(true);

        // 텍스트 스타일링 설정
        SetupTextStyling(success);

        // 포맷된 메시지 설정
        gameOverText.text = FormatGameOverText(success);

        // 텍스트를 처음에는 투명하게
        Color textColor = gameOverText.color;
        textColor.a = 0;
        gameOverText.color = textColor;

        // 배경 페이드인과 함께 텍스트도 페이드인
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / fadeDuration;

            float easedProgress = EaseOutQuart(progress);

            // 배경 페이드인
            Color bgColor = fadePanel.color;
            bgColor.a = easedProgress * 0.85f;
            fadePanel.color = bgColor;

            // 텍스트 페이드인 (배경보다 조금 늦게 시작)
            float textProgress = Mathf.Clamp01((progress - 0.3f) / 0.7f);
            textColor = success ? successColor : failureColor;
            textColor.a = EaseOutQuart(textProgress);
            gameOverText.color = textColor;

            yield return null;
        }

        // 최종 상태 설정
        gameOverText.transform.localScale = Vector3.one;

        yield break;
    }

    float EaseOutQuart(float t)
    {
        return 1f - Mathf.Pow(1f - t, 4f);
    }

    float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    public void RetryGame()
    {
        gameOverActive = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        gameOverActive = false;
        Time.timeScale = 1f;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}