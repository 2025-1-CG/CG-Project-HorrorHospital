using UnityEngine;

public class DoorAutoClose : MonoBehaviour
{
    public Transform leftDoor;   // Left Door
    public Transform rightDoor;  // Right Door
    public float closeDelay = 2f;
    public float closeSpeed = 2f;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;
    private bool shouldClose = false;
    private float timer = 0f;

    void Start()
    {
        // 두 문이 원래 닫혀있는 상태를 기억
        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 수술실에 들어왔습니다. 양쪽 문을 닫습니다.");
            shouldClose = true;
            timer = 0f;
        }
    }

    void Update()
    {
        if (shouldClose)
        {
            timer += Time.deltaTime;

            if (timer >= closeDelay)
            {
                // 부드럽게 양쪽 문이 닫히도록 처리
                leftDoor.localRotation = Quaternion.Slerp(leftDoor.localRotation, leftClosedRotation, Time.deltaTime * closeSpeed);
                rightDoor.localRotation = Quaternion.Slerp(rightDoor.localRotation, rightClosedRotation, Time.deltaTime * closeSpeed);

                // 거의 닫혔다면 완전히 멈추기
                if (Quaternion.Angle(leftDoor.localRotation, leftClosedRotation) < 1f && Quaternion.Angle(rightDoor.localRotation, rightClosedRotation) < 1f)
                {
                    leftDoor.localRotation = leftClosedRotation;
                    rightDoor.localRotation = rightClosedRotation;
                    shouldClose = false;
                    Debug.Log("양쪽 문이 완전히 닫혔습니다.");
                }
            }
        }
    }
}

