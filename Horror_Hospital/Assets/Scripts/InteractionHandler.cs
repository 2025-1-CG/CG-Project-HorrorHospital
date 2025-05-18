// Scripts/Interaction/InteractionHandler.cs
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer; // Interactable 레이어만 감지
    public GameObject interactUI; // "Press E to interact" UI

    private Camera playerCamera;
    private IInteractable currentTarget;

    void Start()
    {
        playerCamera = Camera.main;
        interactUI?.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            currentTarget = hit.collider.GetComponent<IInteractable>();
            if (currentTarget != null)
            {
                interactUI?.SetActive(true);

                if (Input.GetKeyDown(interactKey))
                {
                    currentTarget.Interact();
                }

                return;
            }
        }

        interactUI?.SetActive(false);
        currentTarget = null;
    }
}