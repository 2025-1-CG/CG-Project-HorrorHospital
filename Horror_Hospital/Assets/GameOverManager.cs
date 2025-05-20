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
    public string successMessage = "Ż�� ����!";
    public string failureMessage = "Game Over";

    private void Start()
    {
        // ������ ���� Game Over UI�� ����
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);
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

        // ��ư Ȱ��ȭ
        if (retryButton != null) retryButton.interactable = true;

        yield break;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}