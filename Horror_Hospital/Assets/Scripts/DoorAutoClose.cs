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
        // �� ���� ���� �����ִ� ���¸� ���
        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ �����ǿ� ���Խ��ϴ�. ���� ���� �ݽ��ϴ�.");
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
                // �ε巴�� ���� ���� �������� ó��
                leftDoor.localRotation = Quaternion.Slerp(leftDoor.localRotation, leftClosedRotation, Time.deltaTime * closeSpeed);
                rightDoor.localRotation = Quaternion.Slerp(rightDoor.localRotation, rightClosedRotation, Time.deltaTime * closeSpeed);

                // ���� �����ٸ� ������ ���߱�
                if (Quaternion.Angle(leftDoor.localRotation, leftClosedRotation) < 1f && Quaternion.Angle(rightDoor.localRotation, rightClosedRotation) < 1f)
                {
                    leftDoor.localRotation = leftClosedRotation;
                    rightDoor.localRotation = rightClosedRotation;
                    shouldClose = false;
                    Debug.Log("���� ���� ������ �������ϴ�.");
                }
            }
        }
    }
}

