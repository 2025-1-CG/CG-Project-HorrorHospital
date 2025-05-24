using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameOverManager gameOverManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameOverManager == null)
            gameOverManager = FindObjectOfType<GameOverManager>();
    }

    public void StartGameFlow()
    {
        Debug.Log("게임 시작 플로우 진입");
    }

    public void GameOver(string reason)
    {
        Debug.Log("❌ 게임 오버: " + reason);
        gameOverManager.ShowGameOver(false);
    }

    public void GameClear()
    {
        Debug.Log("🎉 게임 클리어!");
        gameOverManager.ShowGameOver(true);
    }
}