using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{

    [Header("UI Elements")]
    public Canvas gameOverCanvas;
    public Image fadePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subText;

    [Header("Settings")]
    public float fadeDuration = 2f;
    public string successMessage = "GAME CLEAR!";
    public string failureMessage = "GAME OVER";

    [Header("Input Settings")]
    public KeyCode retryKey = KeyCode.R;
    public KeyCode exitKey = KeyCode.Escape;

    [Header("Visual Styling")]
    public Color successColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color failureColor = new Color(0.9f, 0.2f, 0.2f, 1f);
    public Color instructionColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public float textGlowIntensity = 2f;
    public float titlePulseSpeed = 2f;
    public float textShadowOffset = 3f;

    private bool gameOverActive = false;
    private bool isSuccess;

    void Start()
    {
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
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
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.G))
    {
        ShowGameOver(false);
    }
#endif
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.P))
    {
        ShowGameOver(true);
    }
#endif
    }

    public void ShowGameOver(bool success)
    {
        isSuccess = success;
        StartCoroutine(GameOverSequence(success));
    }

    void SetupTextStyling(bool success)
    {
        titleText.text = success ? successMessage : failureMessage;
        subText.text = "[R] RETRY\n[ESC] EXIT";

        titleText.color = success ? successColor : failureColor;
        subText.color = instructionColor;

        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;

        titleText.fontMaterial = CreateTextMaterial(success);

        titleText.outlineWidth = 0.2f;
        titleText.outlineColor = success ?
            new Color(successColor.r, successColor.g, successColor.b, 0.5f) :
            new Color(failureColor.r, failureColor.g, failureColor.b, 0.5f);
    }

    Material CreateTextMaterial(bool success)
    {
        Material newMat = new Material(titleText.fontMaterial);

        newMat.SetFloat("_UnderlayOffsetX", textShadowOffset);
        newMat.SetFloat("_UnderlayOffsetY", -textShadowOffset);
        newMat.SetFloat("_UnderlayDilate", 0.5f);
        newMat.SetColor("_UnderlayColor", new Color(0, 0, 0, 0.8f));

        newMat.SetFloat("_GlowPower", textGlowIntensity);
        newMat.SetColor("_GlowColor", success ? successColor : failureColor);

        return newMat;
    }

    IEnumerator GameOverSequence(bool success)
    {
        gameOverActive = true;
        Time.timeScale = 0f;

        gameOverCanvas.gameObject.SetActive(true);
        SetupTextStyling(success);

        // 초기 투명 설정
        Color titleColor = titleText.color;
        titleColor.a = 0;
        titleText.color = titleColor;

        Color subColor = subText.color;
        subColor.a = 0;
        subText.color = subColor;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / fadeDuration;
            float easedProgress = EaseOutQuart(progress);

            // 배경 페이드 인
            Color bgColor = fadePanel.color;
            bgColor.a = easedProgress * 0.85f;
            fadePanel.color = bgColor;

            // 타이틀 페이드 인
            float titleAlpha = EaseOutQuart(Mathf.Clamp01((progress - 0.2f) / 0.5f));
            titleColor.a = titleAlpha;
            titleText.color = titleColor;

            // 서브 텍스트 페이드 인
            float subAlpha = EaseOutQuart(Mathf.Clamp01((progress - 0.5f) / 0.4f));
            subColor.a = subAlpha;
            subText.color = subColor;

            yield return null;
        }
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