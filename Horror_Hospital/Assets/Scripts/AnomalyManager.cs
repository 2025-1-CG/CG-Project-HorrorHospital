// Scripts/System/AnomalyManager.cs
using UnityEngine;
using System.Collections;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    [Header("이상현상 효과")]
    [SerializeField] private Light[] flickeringLights;
    [SerializeField] private AudioSource anomalySoundSource;
    [SerializeField] private AudioClip[] anomalySounds;
    [SerializeField] private GameObject[] monitors;
    [SerializeField] private Material normalMonitorMaterial;
    [SerializeField] private Material glitchMonitorMaterial;
    
    [Header("제한 시간")]
    [SerializeField] private float anomalyTimeLimit = 60f; // 이상현상 발견 후 버튼 누르기까지 제한시간 (초)

    [Header("카메라 흔들림")]
    [SerializeField] private Camera mainCamera;

    private Vector3 originalCamPos;
    private Coroutine cameraShakeCoroutine;
    private AnomalyType activeAnomaly = AnomalyType.None;
    private Coroutine anomalyCoroutine;
    private bool anomalyActive = false;
    private float anomalyTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // mainCamera가 비어있다면 자동으로 MainCamera할당
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    private void Update()
    {
        // 활성화된 이상현상이 있고 제한시간 카운트다운 중
        if (anomalyActive && activeAnomaly != AnomalyType.None && 
            LoopManager.Instance.currentState == GameState.WaitingForReport)
        {
            anomalyTimer -= Time.deltaTime;
            
            if (anomalyTimer <= 0f)
            {
                // 제한시간이 끝났는데 버튼을 누르지 않은 경우
                Debug.LogWarning("⏰ 제한시간 종료! 버튼을 누르지 않아 게임 오버!");
                GameManager.Instance.GameOver("제한시간 내에 이상현상을 보고하지 않음");
            }
        }
    }

    // LoopManager에서 호출됨
    public void ActivateAnomaly(AnomalyType type)
    {
        Debug.Log($"🔍 ActivateAnomaly: {type}");
        activeAnomaly = type;
        
        // 이전 이상현상 정리
        if (anomalyCoroutine != null)
        {
            StopCoroutine(anomalyCoroutine);
            anomalyCoroutine = null;
        }
        
        ResetAllAnomalies();

        // 이상현상 타입에 따른 연출 처리
        switch (type)
        {
            case AnomalyType.None:
                // 튜토리얼/정상 상태 - 아무 이상 없음
                Debug.Log("정상 상태 - 이상현상 없음");
                break;
                
            case AnomalyType.A:
                Debug.Log("이상현상 A 활성화");
                // A 타입: 모니터 전환, 조명 깜빡임, 이상한 사운드
                anomalyCoroutine = StartCoroutine(PlayAnomalyTypeA());
                break;
                
            case AnomalyType.B:
                Debug.Log("이상현상 B 활성화");
                // TODO: B 타입 이상현상 연출
                break;
                
            case AnomalyType.C:
                Debug.Log("이상현상 C 활성화");
                // TODO: C 타입 이상현상 연출
                break;
        }
    }
    
    // 모든 이상현상 효과 리셋
    private void ResetAllAnomalies()
    {
        anomalyActive = false;
        anomalyTimer = anomalyTimeLimit;
        
        // 조명 원상복구
        if (flickeringLights != null)
        {
            foreach (Light light in flickeringLights)
            {
                if (light != null)
                {
                    light.enabled = true;
                }
            }
        }

        // 카메라 흔들림 중단
        if (cameraShakeCoroutine != null)
        {
            StopCoroutine(cameraShakeCoroutine);
            cameraShakeCoroutine = null;
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCamPos;
            }
        }

        // 모니터 원상복구
        if (monitors != null && normalMonitorMaterial != null)
        {
            foreach (GameObject monitor in monitors)
            {
                if (monitor != null)
                {
                    Renderer renderer = monitor.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = normalMonitorMaterial;
                    }
                }
            }
        }
        
        // 사운드 중지
        if (anomalySoundSource != null)
        {
            anomalySoundSource.Stop();
        }
    }
    
    // 이상현상 타입 A: 모니터 전환, 조명 깜빡임, 이상한 사운드
    private IEnumerator PlayAnomalyTypeA()
    {
        anomalyActive = false; // 트리거에 들어가기 전에는 비활성화 상태
        
        // 플레이어가 트리거 영역에 들어가야 활성화
        yield return new WaitUntil(() => anomalyActive);
        
        Debug.Log("🖥️ 이상현상 A가 트리거되었습니다!");
        anomalyTimer = anomalyTimeLimit; // 타이머 리셋
        
        // 1. 모니터 화면 전환
        if (monitors != null && glitchMonitorMaterial != null)
        {
            foreach (GameObject monitor in monitors)
            {
                if (monitor != null)
                {
                    Renderer renderer = monitor.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = glitchMonitorMaterial;
                    }
                }
            }
        }
        
        // 2. 사운드 재생
        if (anomalySoundSource != null && anomalySounds != null && anomalySounds.Length > 0)
        {
            AudioClip sound = anomalySounds[Random.Range(0, anomalySounds.Length)];
            if (sound != null)
            {
                anomalySoundSource.clip = sound;
                anomalySoundSource.loop = true;
                anomalySoundSource.Play();
            }

            // 카메라 흔들림
            if (mainCamera != null)
            {
                if (cameraShakeCoroutine != null)
                {
                    StopCoroutine(cameraShakeCoroutine);
                }
                cameraShakeCoroutine = StartCoroutine(CameraShake(60f, 0.05f));
            }


        }

        // 3. 조명 깜빡임
        while (anomalyActive)
        {
            if (flickeringLights != null)
            {
                foreach (Light light in flickeringLights)
                {
                    if (light != null)
                    {
                        light.enabled = !light.enabled;
                    }
                }
            }
            
            // 깜빡임 주기 - 랜덤
            float flickerTime = Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(flickerTime);
        }
    }
    
    // 이상현상 트리거 영역에서 호출됨
    public void OnPlayerEnteredAnomalyZone()
    {
        if (activeAnomaly != AnomalyType.None)
        {
            anomalyActive = true;
            Debug.Log("⚠️ 플레이어가 이상현상 구역에 들어왔습니다. 타이머 시작!");
        }
    }
    
    // 게임 오버나 루프 종료 시 호출
    public void StopAllAnomalies()
    {
        if (anomalyCoroutine != null)
        {
            StopCoroutine(anomalyCoroutine);
            anomalyCoroutine = null;
        }
        
        ResetAllAnomalies();
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        originalCamPos = mainCamera.transform.localPosition;

        while (elapsed < duration)
        {
            // 현재까지 경과한 비율
            float percentComplete = elapsed / duration;

            // 흔들림 세기를 시간에 따라 감소시키기
            float currentMagnitude = Mathf.Lerp(magnitude, 0f, percentComplete);

            // 랜덤 위치 오프셋 계산
            float offsetX = Random.Range(-1f, 1f) * currentMagnitude;
            float offsetY = Random.Range(-1f, 1f) * currentMagnitude;

            // 카메라 위치 적용
            mainCamera.transform.localPosition = originalCamPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 흔들림 종료 후 원위치 복구
        mainCamera.transform.localPosition = originalCamPos;
    }


}