using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer;

    [Header("Interaction UIs")]
    public GameObject anomalyUI;      // 이상 있음 버튼 UI
    public GameObject noAnomalyUI;    // 이상 없음 버튼 UI
    public GameObject waitUI;

    private Camera playerCamera;
    private IInteractable currentTarget;


    void Start()
    {
        playerCamera = Camera.main;
        HideAllUIs();
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
                ShowUIBasedOnTarget(currentTarget);

                if (!LoopManager.Instance.IsButtonLocked && Input.GetKeyDown(interactKey))
                {
                    currentTarget.Interact();
                }

                return;
            }
        }

        HideAllUIs();
        currentTarget = null;
    }
    void ShowUIBasedOnTarget(IInteractable target)
    {
        HideAllUIs();

        if (LoopManager.Instance.IsButtonLocked)
        {
            waitUI?.SetActive(true);
            return;
        }

        if (target is AnomalyButton button)
        {
            if (button.IsAnomaly)
                anomalyUI?.SetActive(true);
            else
                noAnomalyUI?.SetActive(true);
        }
    }

    void HideAllUIs()
    {
        anomalyUI?.SetActive(false);
        noAnomalyUI?.SetActive(false);
        waitUI?.SetActive(false);
    }
}