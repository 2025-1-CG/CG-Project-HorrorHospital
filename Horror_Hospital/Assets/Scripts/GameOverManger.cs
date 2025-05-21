using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Canvas gameOverCanvas;
    public UnityEngine.UI.Image fadePanel;
    public TextMeshProUGUI gameOverText;
    public Button retryButton;

    [Header("Settings")]
    public float fadeDuration = 2f;
    public string successMessage = "Game Clear!";
    public string failureMessage = "Game Over";

    private void Start()
    {
        // 시작할 때는 Game Over UI를 숨김
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);

        // 시작 시 버튼 비활성화
        if (retryButton != null) retryButton.interactable = false;
    }

    public void ShowGameOver(bool isSuccess)
    {
        StartCoroutine(GameOverSequence(isSuccess));
    }

    private IEnumerator GameOverSequence(bool isSuccess)
    {
        // 게임 일시정지
        Time.timeScale = 0f;

        // Canvas 활성화
        gameOverCanvas.gameObject.SetActive(true);

        // 메시지 설정
        gameOverText.text = isSuccess ? successMessage : failureMessage;

        // 페이드인 효과
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha * 0.8f); // 80% 투명도

            // 텍스트도 함께 페이드인
            Color textColor = gameOverText.color;
            textColor.a = alpha;
            gameOverText.color = textColor;

            yield return null;
        }

        // 버튼 페이드인 효과
        yield return StartCoroutine(FadeInButton());

        yield break;
    }

    private IEnumerator FadeInButton()
    {
        if (retryButton != null)
        {
            // 알파값 0부터 시작
            CanvasGroup buttonGroup = retryButton.GetComponent<CanvasGroup>();
            if (buttonGroup == null)
                buttonGroup = retryButton.gameObject.AddComponent<CanvasGroup>();

            buttonGroup.alpha = 0;

            // 페이드인
            float buttonFadeTime = 0.5f;
            float buttonElapsedTime = 0f;

            while (buttonElapsedTime < buttonFadeTime)
            {
                buttonElapsedTime += Time.unscaledDeltaTime;
                float buttonAlpha = Mathf.Clamp01(buttonElapsedTime / buttonFadeTime);
                buttonGroup.alpha = buttonAlpha;

                yield return null;
            }

            retryButton.interactable = true;
        }
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}