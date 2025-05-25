// Scripts/Interaction/AnomalyButton.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnomalyButton : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isAnomaly = false;
    [SerializeField] private AudioClip clickSound;

    public bool IsAnomaly => isAnomaly;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    public void Interact()
    {
        // 사운드 재생
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        // 로그
        Debug.Log($"[Button Clicked] {(isAnomaly ? "🟥 Anomaly Detected" : "🟩 No Anomaly")}");

        // 이상현상 보고
        LoopManager.Instance.ReportAnomaly(isAnomaly);
    }
}