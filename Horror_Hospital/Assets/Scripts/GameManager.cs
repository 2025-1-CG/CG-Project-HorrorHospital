using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver(string reason)
    {
        Debug.Log("❌ 게임 오버: " + reason);
        // TODO: UI, 리셋, 페이드 등
    }

    public void GameClear()
    {
        Debug.Log("🎉 게임 클리어!");
        // TODO: 성공 UI 띄우기
    }
}