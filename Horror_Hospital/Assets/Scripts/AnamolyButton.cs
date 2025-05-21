// Scripts/Interaction/AnomalyButton.cs
using UnityEngine;

public class AnomalyButton : MonoBehaviour, IInteractable
{
    public bool isGlitchButton = false; // true: 이상 있음, false: 이상 없음
    
    [SerializeField]
    private string buttonDescription;  // 버튼 설명 (예: "이상 없음", "이상 있음")

    [SerializeField]
    private AudioClip buttonSound;     // 버튼 클릭 사운드

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Interact()
    {
        // 버튼 사운드 재생
        if (buttonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        Debug.Log($"버튼 클릭됨: {(isGlitchButton ? "이상 있음" : "이상 없음")}");
        LoopManager.Instance.ReportAnomaly(isGlitchButton);
    }
}