using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnomalyType { None, A, B, C }
public enum GameState { Waiting, InProgress, WaitingForReport, WaitingForExit }

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public int loopCount = 0;
    public int maxLoop = 4;
    public AnomalyType currentAnomaly = AnomalyType.None;
    public bool anomalyReported = false;
    public GameState currentState = GameState.Waiting;

    private List<AnomalyType> loopAnomalies = new List<AnomalyType>();

    [Header("트리거 영역")]
    [SerializeField] private AnomalyTriggerZone[] anomalyTriggerZones;

    [Header("디버그")]
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitAnomalySequence();
    }

    // 문이 완전히 닫혔을 때 호출됨 (DoorController에서 호출)
    public void OnDoorClosed()
    {
        if (currentState == GameState.Waiting)
        {
            if (loopCount == 0)
            {
                Debug.Log("문이 닫혔습니다. 튜토리얼 루프를 시작합니다.");
                StartLoop();
            }
            else
            {
                Debug.Log($"문이 닫혔습니다. 루프 {loopCount}를 시작합니다.");
                StartNextLoop();
            }
        }
    }

    private void InitAnomalySequence()
    {
        loopAnomalies.Clear();

        // 2개는 튜토리얼(None)
        loopAnomalies.Add(AnomalyType.None);
        loopAnomalies.Add(AnomalyType.None);

        // 3개는 랜덤 A/B/C
        List<AnomalyType> anomalies = new List<AnomalyType> {
        AnomalyType.A,
        AnomalyType.B,
        AnomalyType.C
    };

        for (int i = 0; i < 10; i++) // 셔플
        {
            int i1 = Random.Range(0, anomalies.Count);
            int i2 = Random.Range(0, anomalies.Count);
            (anomalies[i1], anomalies[i2]) = (anomalies[i2], anomalies[i1]);
        }

        loopAnomalies.AddRange(anomalies);

        // 전체 셔플 (튜토리얼과 이상현상 뒤섞기)
        for (int i = 0; i < 10; i++)
        {
            int i1 = Random.Range(0, loopAnomalies.Count);
            int i2 = Random.Range(0, loopAnomalies.Count);
            (loopAnomalies[i1], loopAnomalies[i2]) = (loopAnomalies[i2], loopAnomalies[i1]);
        }

        Debug.Log("✅ 루프 순서: " + string.Join(", ", loopAnomalies));
    }

    public void StartLoop()
    {
        loopCount = 0;
        currentState = GameState.InProgress;
        Debug.Log("🎮 게임 시작");
        StartNextLoop();
    }

    public void StartNextLoop()
    {
        currentState = GameState.InProgress;
        anomalyReported = false;

        // 모든 이상현상 트리거 영역 리셋
        ResetAllTriggerZones();

        SelectAnomaly();
    }

    private void SelectAnomaly()
    {
        if (loopCount >= loopAnomalies.Count)
        {
            Debug.LogWarning("⚠ 루프 범위 초과");
            currentAnomaly = AnomalyType.None;
        }
        else
        {
            currentAnomaly = loopAnomalies[loopCount];
        }

        Debug.Log($"[Loop {loopCount}] 이상현상: {currentAnomaly}");
        AnomalyManager.Instance.ActivateAnomaly(currentAnomaly);
        currentState = GameState.WaitingForReport;
    }

    public void ReportAnomaly(bool reportedAsGlitch)
    {
        if (currentState != GameState.WaitingForReport)
        {
            Debug.LogWarning("잘못된 상태에서 버튼이 눌렸습니다.");
            return;
        }

        anomalyReported = true;
        currentState = GameState.WaitingForExit;

        // 이상현상 효과 정지
        AnomalyManager.Instance.StopAllAnomalies();

        bool correct =
            (currentAnomaly == AnomalyType.None && !reportedAsGlitch) ||
            (currentAnomaly != AnomalyType.None && reportedAsGlitch);

        if (!correct)
        {
            GameManager.Instance.GameOver("판단 오류로 인해 게임 오버!");
        }
        else
        {
            string message = loopCount == 0
                ? "✅ 튜토리얼 성공! 복도로 퇴장하세요."
                : "✅ 정확하게 보고됨. 복도로 퇴장하세요.";

            Debug.Log(message);

            // TODO: UI로 플레이어에게 알림
        }
    }

    // 복도 출구 트리거에서 호출됨
    public void ExitToHallway()
    {
        if (currentState != GameState.WaitingForExit)
        {
            Debug.LogWarning("⚠ 버튼을 누르지 않고 퇴장하려 했습니다 → 게임 오버");
            GameManager.Instance.GameOver("버튼을 누르지 않고 퇴장함");
            return;
        }

        loopCount++;
        Debug.Log($"🚶 복도로 퇴장 완료. 다음 루프({loopCount})를 위해 다시 입장하세요.");

        currentState = GameState.Waiting;

        if (loopCount > maxLoop)
        {
            GameManager.Instance.GameClear();
        }
    }

    // 모든 이상현상 트리거 영역 리셋
    private void ResetAllTriggerZones()
    {
        if (anomalyTriggerZones != null)
        {
            foreach (AnomalyTriggerZone zone in anomalyTriggerZones)
            {
                if (zone != null)
                {
                    zone.ResetTrigger();
                }
            }
        }
    }

    // 디버그용 메서드
    public void DebugSkipToNextLoop()
    {
        if (!debugMode) return;

        Debug.Log("🛠 디버그: 다음 루프로 강제 이동");
        if (currentState == GameState.WaitingForExit)
        {
            ExitToHallway();
        }
        else
        {
            ReportAnomaly(currentAnomaly != AnomalyType.None);
            ExitToHallway();
        }
    }
}