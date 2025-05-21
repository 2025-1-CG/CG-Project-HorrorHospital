using UnityEngine;

public class AnomalyTriggerZone : MonoBehaviour
{
    [SerializeField] private AnomalyType triggerAnomalyType = AnomalyType.A;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.2f, 0.2f, 0.3f);
    
    private bool wasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (wasTriggered) return; // 이미 트리거됨
        
        if (other.CompareTag(playerTag))
        {
            // 현재 활성화된 이상현상이 내 타입과 일치하는지 확인
            if (LoopManager.Instance.currentAnomaly == triggerAnomalyType)
            {
                Debug.Log($"🚨 트리거 영역 진입! 이상현상 {triggerAnomalyType} 활성화");
                wasTriggered = true;
                
                // AnomalyManager에 플레이어가 영역에 들어왔음을 알림
                AnomalyManager.Instance.OnPlayerEnteredAnomalyZone();
            }
        }
    }
    
    // 루프 시작 시 리셋
    public void ResetTrigger()
    {
        wasTriggered = false;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // 트리거 영역 시각화
        Gizmos.color = gizmoColor;
        
        // 콜라이더 타입에 따라 다른 모양으로 그림
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
        
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
        }
    }
} 