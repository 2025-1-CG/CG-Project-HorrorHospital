using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class RigidbodyFPSController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float pushPower = 2.0f; // 밀 때 전달하는 힘

    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;

    private Rigidbody rb;
    private AudioSource audioSource;
    private float footstepTimer = 0f;
    private bool isMoving = false;
    private float rotationX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D 사운드

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
  

    // 걷는 소리 처리
    isMoving = move.magnitude > 0.1f;
        if (isMoving)
        {
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
    footstepTimer = footstepInterval;
            }
        }
        else
{
    footstepTimer = 0f; // 멈추면 타이머 초기화
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // 이동 멈추면 소리 끄기
            }
        }
}

    void PlayFootstep()
{
    if (footstepClips.Length == 0) return;

    int index = Random.Range(0, footstepClips.Length);
    audioSource.clip = footstepClips[index];
    audioSource.Play();
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
