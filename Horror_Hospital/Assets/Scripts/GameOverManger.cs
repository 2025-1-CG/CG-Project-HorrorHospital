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
        // ������ ���� Game Over UI�� ����
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);

        // ���� �� ��ư ��Ȱ��ȭ
        if (retryButton != null) retryButton.interactable = false;
    }

    public void ShowGameOver(bool isSuccess)
    {
        StartCoroutine(GameOverSequence(isSuccess));
    }

    private IEnumerator GameOverSequence(bool isSuccess)
    {
        // ���� �Ͻ�����
        Time.timeScale = 0f;

        // Canvas Ȱ��ȭ
        gameOverCanvas.gameObject.SetActive(true);

        // �޽��� ����
        gameOverText.text = isSuccess ? successMessage : failureMessage;

        // ���̵��� ȿ��
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha * 0.8f); // 80% ����

            // �ؽ�Ʈ�� �Բ� ���̵���
            Color textColor = gameOverText.color;
            textColor.a = alpha;
            gameOverText.color = textColor;

            yield return null;
        }

        // ��ư ���̵��� ȿ��
        yield return StartCoroutine(FadeInButton());

        yield break;
    }

    private IEnumerator FadeInButton()
    {
        if (retryButton != null)
        {
            // ���İ� 0���� ����
            CanvasGroup buttonGroup = retryButton.GetComponent<CanvasGroup>();
            if (buttonGroup == null)
                buttonGroup = retryButton.gameObject.AddComponent<CanvasGroup>();

            buttonGroup.alpha = 0;

            // ���̵���
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