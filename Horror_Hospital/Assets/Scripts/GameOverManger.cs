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
    public Color successColor = new Color(0.2f, 0.8f, 0.2f, 1f); // ���� �ʷ�
    public Color failureColor = new Color(0.9f, 0.2f, 0.2f, 1f); // ���� ����
    public Color instructionColor = new Color(0.8f, 0.8f, 0.8f, 1f); // ���� ȸ��
    public float textGlowIntensity = 2f;
    public float titlePulseSpeed = 2f;
    public float textShadowOffset = 3f;

    private bool gameOverActive = false;
    private Color originalTextColor;
    private bool isSuccess;

    void Start()
    {
        // ������ ���� Game Over UI�� ����
        gameOverCanvas.gameObject.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);

        // ���� �ؽ�Ʈ ���� ����
        if (gameOverText != null)
            originalTextColor = gameOverText.color;
    }

    void Update()
    {
        // ���ӿ��� ���¿��� Ű���� �Է� ó��
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

        // �ؽ�Ʈ ũ��� ����
        gameOverText.fontSize = success ? 72f : 64f;
        gameOverText.fontStyle = FontStyles.Bold;
        gameOverText.alignment = TextAlignmentOptions.Center;

        // �׸��� ȿ��
        gameOverText.fontMaterial = CreateTextMaterial(success);

        // �ؽ�Ʈ ����
        gameOverText.color = success ? successColor : failureColor;

        // �۷ο� ȿ���� ���� Outline ����
        gameOverText.outlineWidth = 0.2f;
        gameOverText.outlineColor = success ?
            new Color(successColor.r, successColor.g, successColor.b, 0.5f) :
            new Color(failureColor.r, failureColor.g, failureColor.b, 0.5f);
    }

    Material CreateTextMaterial(bool success)
    {
        Material newMat = new Material(gameOverText.fontMaterial);

        // ������� ����
        newMat.SetFloat("_UnderlayOffsetX", textShadowOffset);
        newMat.SetFloat("_UnderlayOffsetY", -textShadowOffset);
        newMat.SetFloat("_UnderlayDilate", 0.5f);
        newMat.SetColor("_UnderlayColor", new Color(0, 0, 0, 0.8f));

        // �۷ο� ȿ��
        newMat.SetFloat("_GlowPower", textGlowIntensity);
        newMat.SetColor("_GlowColor", success ? successColor : failureColor);

        return newMat;
    }

    string FormatGameOverText(bool success)
    {
        // ���� or ���� �޽���
        string mainMessage = success ? successMessage : failureMessage;
        string formattedMain = $"<size=120%><b>{mainMessage}</b></size>";

        // ����� or ���� ����
        string instructions = $"\n\n<size=60%><color=#{ColorUtility.ToHtmlStringRGB(instructionColor)}>" +
                            $"<b>[R]</b> RETRY GAME\n" +
                            $"<b>[Esc]</b> EXIT GAME\n" +
                            $"</color></size>";

        return formattedMain + instructions;
    }

    IEnumerator GameOverSequence(bool success)
    {
        gameOverActive = true;

        // ���� �Ͻ�����
        Time.timeScale = 0f;

        // Canvas Ȱ��ȭ
        gameOverCanvas.gameObject.SetActive(true);

        // �ؽ�Ʈ ��Ÿ�ϸ� ����
        SetupTextStyling(success);

        // ���˵� �޽��� ����
        gameOverText.text = FormatGameOverText(success);

        // �ؽ�Ʈ�� ó������ �����ϰ�
        Color textColor = gameOverText.color;
        textColor.a = 0;
        gameOverText.color = textColor;

        // ��� ���̵��ΰ� �Բ� �ؽ�Ʈ�� ���̵���
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / fadeDuration;

            float easedProgress = EaseOutQuart(progress);

            // ��� ���̵���
            Color bgColor = fadePanel.color;
            bgColor.a = easedProgress * 0.85f;
            fadePanel.color = bgColor;

            // �ؽ�Ʈ ���̵��� (��溸�� ���� �ʰ� ����)
            float textProgress = Mathf.Clamp01((progress - 0.3f) / 0.7f);
            textColor = success ? successColor : failureColor;
            textColor.a = EaseOutQuart(textProgress);
            gameOverText.color = textColor;

            yield return null;
        }

        // ���� ���� ����
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