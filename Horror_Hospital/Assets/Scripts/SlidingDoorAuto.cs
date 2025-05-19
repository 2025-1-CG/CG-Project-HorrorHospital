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

    private Vector3 leftClosedPos, rightClosedPos;
    private Vector3 leftOpenPos, rightOpenPos;

    private bool shouldClose = false;
    private float timer = 0f;
    private bool hasEnteredThisLoop = false;
    private AudioSource audioSource;

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
        else if (!isEntering && LoopManager.Instance.waitingForExit)
        {
            Debug.Log("▶ 퇴장 → 문 열림");
            OpenDoors();
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
                Debug.Log("🚪 문이 닫혔습니다.");
            }
        }
    }

    public void OpenDoors()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(leftDoor, leftOpenPos));
        StartCoroutine(SlideTo(rightDoor, rightOpenPos));
    }

    public void CloseDoors()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(leftDoor, leftClosedPos));
        StartCoroutine(SlideTo(rightDoor, rightClosedPos));

        // 문 닫는 소리 재생
        if (closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }
    }

    private System.Collections.IEnumerator SlideTo(Transform door, Vector3 target)
    {
        while (Vector3.Distance(door.localPosition, target) > 0.01f)
        {
            door.localPosition = Vector3.Lerp(door.localPosition, target, Time.deltaTime * slideSpeed);
            yield return null;
        }

        door.localPosition = target; // Snap 정렬
    }

    public void ResetDoor()
    {
        // 다음 루프 시작 시 호출
        hasEnteredThisLoop = false;
        CloseDoors();
    }
    private System.Collections.IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseDoors();
    }
}