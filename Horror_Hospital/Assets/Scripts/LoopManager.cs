using UnityEngine;
using System.Collections;

public enum AnomalyType { None, A, B, C }
public enum GameState { Waiting, InProgress, WaitingForReport, WaitingForExit }

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public int loopCount = 0;
    public int maxLoop = 3;
    public AnomalyType currentAnomaly = AnomalyType.None;
    public bool anomalyReported = false;
    public GameState currentState = GameState.Waiting;

    [Header("트리거 영역")]
    [SerializeField] private AnomalyTriggerZone[] anomalyTriggerZones;
    
    [Header("디버그")]
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 문이 완전히 닫혔을 때 호출됨 (DoorController에서 호출)
    public void OnDoorClosed()
    {
        if (currentState == GameState.Waiting)
        {
            if (loopCount == 0)
            {
                Debug.Log("문이 닫혔습니다. 튜토리얼 루프를 시작합니다.");
                StartGame();
            }
            else
            {
                Debug.Log($"문이 닫혔습니다. 루프 {loopCount}를 시작합니다.");
                StartNextLoop();
            }
        }
    }

    public void StartGame()
    {
        loopCount = 0; // 0번째는 튜토리얼 루프
        currentState = GameState.InProgress;
        
        Debug.Log("🎮 게임 시작: 튜토리얼 루프");
        StartCoroutine(StartTutorialLoop());
    }

    private IEnumerator StartTutorialLoop()
    {
        // 모든 이상현상 트리거 영역 리셋
        ResetAllTriggerZones();
        
        // 잠시 대기 후 튜토리얼 루프 시작 (플레이어가 상황을 인지할 수 있도록)
        yield return new WaitForSeconds(1.5f);
        
        currentAnomaly = AnomalyType.None;
        AnomalyManager.Instance.ActivateAnomaly(currentAnomaly);
        
        currentState = GameState.WaitingForReport;
        Debug.Log("🔍 튜토리얼: 이상현상이 없습니다. 이상 없음 버튼을 누르세요.");
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
        if (loopCount == 0)
        {
            currentAnomaly = AnomalyType.None; // 튜토리얼은 항상 이상없음
        }
        else
        {
            // 순차적으로 이상현상 타입 설정 (None → A → B → C)
            switch (loopCount)
            {
                case 1:
                    currentAnomaly = AnomalyType.A;
                    break;
                case 2:
                    currentAnomaly = AnomalyType.B;
                    break;
                case 3:
                default:
                    currentAnomaly = AnomalyType.C;
                    break;
            }
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