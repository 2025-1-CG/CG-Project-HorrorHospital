using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SlidingDoorAuto : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public float slideDistance = 1.5f;   // 문이 이동할 거리
    public float slideSpeed = 3f;
    public float closeDelay = 2f;

    [SerializeField] private Transform doorFacingForward;
    public AudioClip closeSound;
    public AudioClip openSound;

    private Vector3 leftClosedPos, rightClosedPos;
    private Vector3 leftOpenPos, rightOpenPos;

    private bool shouldClose = false;
    private float timer = 0f;
    private bool hasEnteredThisLoop = false;
    private AudioSource audioSource;
    private bool isFullyClosed = true;
    private bool isClosingAfterExit = false; // 퇴장 후 문이 닫히는 중인지 추적

    void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        // 기준 방향 따라 자동 계산 (왼쪽은 -x, 오른쪽은 +x로 열림)
        leftOpenPos = leftClosedPos + Vector3.left * slideDistance;
        rightOpenPos = rightClosedPos + Vector3.right * slideDistance;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D 사운드
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (doorFacingForward == null)
        {
            Debug.LogError("❌ doorFacingForward가 할당되지 않았습니다!");
            return;
        }

        Vector3 dir = (other.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(doorFacingForward.forward, dir);
        bool isEntering = dot < 0f;

        Debug.Log($"[문 방향 감지] dot = {dot}");

        if (isEntering)
        {
            // 입장 시에는 퇴장 후 닫힘 상태를 리셋
            isClosingAfterExit = false;
            
            if (!hasEnteredThisLoop)
            {
                Debug.Log("▶ 수술실 첫 입장 → 문 열림 후 닫힘 예약");
                OpenDoors();
                timer = 0f;
                shouldClose = true;
                hasEnteredThisLoop = true;
            }
            else
            {
                Debug.Log("🔄 재입장 → 문 다시 열림 후 닫힘 예약");
                OpenDoors();
                timer = 0f;
                shouldClose = true;
            }
        }
        else if (!isEntering && LoopManager.Instance.currentState == GameState.WaitingForExit)
        {
            Debug.Log("▶ 퇴장 → 문 열림");
            OpenDoors();
            
            // 퇴장 표시
            isClosingAfterExit = true;
            
            // 복도로 퇴장 처리
            LoopManager.Instance.ExitToHallway();

            StartCoroutine(CloseAfterDelay(closeDelay));
        }
    }

    void Update()
    {
        if (shouldClose)
        {
            timer += Time.deltaTime;
            if (timer >= closeDelay)
            {
                CloseDoors();
                shouldClose = false;
                Debug.Log("🚪 문이 닫히는 중...");
            }
        }
    }

    public void OpenDoors()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(leftDoor, leftOpenPos));
        StartCoroutine(SlideTo(rightDoor, rightOpenPos));
        isFullyClosed = false;

        // 문 여는 소리 재생
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }

    public void CloseDoors()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(leftDoor, leftClosedPos, OnDoorsFullyClosed));
        StartCoroutine(SlideTo(rightDoor, rightClosedPos));

        // 문 닫는 소리 재생
        if (closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }
    }

    private System.Collections.IEnumerator SlideTo(Transform door, Vector3 target, System.Action onComplete = null)
    {
        while (Vector3.Distance(door.localPosition, target) > 0.01f)
        {
            door.localPosition = Vector3.Lerp(door.localPosition, target, Time.deltaTime * slideSpeed);
            yield return null;
        }

        door.localPosition = target; // Snap 정렬
        
        if (onComplete != null)
        {
            onComplete();
        }
    }

    private void OnDoorsFullyClosed()
    {
        if (!isFullyClosed)
        {
            isFullyClosed = true;
            Debug.Log("🚪 문이 완전히 닫혔습니다.");
            
            // 퇴장 후 문 닫힘이 아닌 경우에만 다음 루프 시작
            if (!isClosingAfterExit && LoopManager.Instance.currentState == GameState.Waiting)
            {
                Debug.Log("입장 후 문이 닫혔습니다. 루프를 시작합니다.");
                LoopManager.Instance.OnDoorClosed();
            }
            else if (isClosingAfterExit)
            {
                Debug.Log("퇴장 후 문이 닫혔습니다. 다음 입장을 기다립니다.");
                // 퇴장 후 문 닫힘 상태 리셋은 입장 시 처리
            }
        }
    }

    public void ResetDoor()
    {
        // 다음 루프 시작 시 호출
        hasEnteredThisLoop = false;
        isClosingAfterExit = false;
        CloseDoors();
    }
    
    private System.Collections.IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseDoors();
    }
}