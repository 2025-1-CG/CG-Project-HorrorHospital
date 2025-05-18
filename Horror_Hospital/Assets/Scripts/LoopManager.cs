using UnityEngine;

public enum AnomalyType { None, A, B, C }

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public int loopCount = 0;
    public int maxLoop = 3;
    public AnomalyType currentAnomaly = AnomalyType.None;
    public bool anomalyReported = false;
    public bool waitingForExit = false; // 버튼 누른 후 퇴장 대기

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        loopCount = 1;
        SelectAnomaly();
        waitingForExit = false;
    }

    public void SelectAnomaly()
    {
        anomalyReported = false;

        if (loopCount == 1)
            currentAnomaly = AnomalyType.None;
        else
            currentAnomaly = (AnomalyType)Random.Range(1, 4); // A~C 중 랜덤

        Debug.Log($"[Loop {loopCount}] 이상현상: {currentAnomaly}");
        AnomalyManager.Instance.ActivateAnomaly(currentAnomaly);
    }

    public void ReportAnomaly(bool reportedAsGlitch)
    {
        anomalyReported = true;
        waitingForExit = true;

        bool correct =
            (currentAnomaly == AnomalyType.None && !reportedAsGlitch) ||
            (currentAnomaly != AnomalyType.None && reportedAsGlitch);

        if (!correct)
        {
            GameManager.Instance.GameOver("판단 오류로 인해 게임 오버!");
        }
        else
        {
            Debug.Log("정확하게 보고됨. 복도로 퇴장하세요.");
        }
    }

    public void ExitToHallway()
    {
        if (!waitingForExit)
        {
            Debug.Log("⚠ 버튼을 누르지 않고 퇴장하려 했습니다 → 게임 오버");
            GameManager.Instance.GameOver("버튼을 누르지 않고 퇴장함");
            return;
        }

        loopCount++;

        if (loopCount > maxLoop)
        {
            GameManager.Instance.GameClear();
        }
        else
        {
            waitingForExit = false;
            SelectAnomaly();
        }
    }
}