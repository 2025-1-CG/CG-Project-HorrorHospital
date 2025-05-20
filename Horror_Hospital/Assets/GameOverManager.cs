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
    public Button retryButton;

    [Header("Settings")]
    public float fadeDuration = 2f;
    public string successMessage = "탈출 성공!";
    public string failureMessage = "Game Over";

    private void Start()
    {
        // 시작할 때는 Game Over UI를 숨김
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);
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

        // 버튼 활성화
        if (retryButton != null) retryButton.interactable = true;

        yield break;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}