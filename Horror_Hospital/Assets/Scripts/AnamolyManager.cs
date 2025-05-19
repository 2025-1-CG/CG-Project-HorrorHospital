// Scripts/System/AnomalyManager.cs
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivateAnomaly(AnomalyType type)
    {
        Debug.Log($"🔍 ActivateAnomaly: {type}");

        // TODO: 연출 연동 (모니터 전환, 조명, 사운드 등)
        // 일단은 콘솔에 로그만 찍음
    }
}