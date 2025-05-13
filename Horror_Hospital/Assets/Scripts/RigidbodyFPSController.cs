using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyFPSController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float pushPower = 2.0f; // 밀 때 전달하는 힘

    private Rigidbody rb;
    private float rotationX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        RotateView();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    // 충돌 시 문을 밀어주는 함수
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody body = collision.rigidbody;

        if (body != null && !body.isKinematic)
        {
            Vector3 pushDir = collision.contacts[0].normal * -1; // 밀리는 방향
            body.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }
    }
}
