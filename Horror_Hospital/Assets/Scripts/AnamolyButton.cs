// Scripts/Interaction/AnomalyButton.cs
using UnityEngine;

public class AnomalyButton : MonoBehaviour, IInteractable
{
    public bool isGlitchButton = false; // true: 이상 있음, false: 이상 없음

    public void Interact()
    {
        Debug.Log("버튼 클릭됨: " + (isGlitchButton ? "이상 있음" : "이상 없음"));
        LoopManager.Instance.ReportAnomaly(isGlitchButton);
    }
}