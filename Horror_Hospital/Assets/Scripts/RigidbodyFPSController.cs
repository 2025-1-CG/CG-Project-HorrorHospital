using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class RigidbodyFPSController : MonoBehaviour
{
    public bool canControl = true;
    public float moveSpeed = 1.68f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float pushPower = 2.0f; // �� �� �����ϴ� ��

    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;

    private Rigidbody rb;
    private AudioSource audioSource;
    private float footstepTimer = 0f;
    private bool isMoving = false;
    private float rotationX = 0f;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponentInChildren<Animator>();

        // AudioSource �ʱ�ȭ
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D ����

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canControl) return;
        RotateView();
    }

    void FixedUpdate()
    {
        if (!canControl) return;
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
        float moveAmount = move.magnitude;
        if (move.magnitude > 0.01f)
        {
            animator.SetFloat("Speed", move.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);

        // �ȴ� �Ҹ� ó��
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
            footstepTimer = 0f; // ���߸� Ÿ�̸� �ʱ�ȭ
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // �̵� ���߸� �Ҹ� ����
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

    // �浹 �� ���� �о��ִ� �Լ�
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody body = collision.rigidbody;

        if (body != null && !body.isKinematic)
        {
            Vector3 pushDir = collision.contacts[0].normal * -1; // �и��� ����
            body.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }
    }
}
