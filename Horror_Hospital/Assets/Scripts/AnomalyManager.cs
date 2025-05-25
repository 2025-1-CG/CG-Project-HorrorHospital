// Scripts/System/AnomalyManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;
    private Coroutine dialogueCoroutine;

    [Header("이상현상 효과")]
    [SerializeField] private Light[] flickeringLights;
    [SerializeField] private AudioSource anomalySoundSource;
    [SerializeField] private AudioClip[] anomalySounds;
    [SerializeField] private AudioSource anomalyASoundSource;
    [SerializeField] private AudioClip anomalyASoundClip;
    [SerializeField] private GameObject[] monitors;
    [SerializeField] private Material normalMonitorMaterial;
    [SerializeField] private Material glitchMonitorMaterial;

    [Header("이상현상 UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private float dialogueFadeDuration = 1f;
    [SerializeField] private float dialogueShowDuration = 3f;
    [SerializeField] private TypewriterEffect typewriterEffect;

    [Header("이상현상 B - 환자 복제")]
    [SerializeField] private GameObject patientPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxPatientCount = 5;
    [SerializeField] private AudioSource breathingSource;
    [SerializeField] private AudioClip breathingClip;

    [Header("이상현상 C - 붉은 물 상승")]
    [SerializeField] private GameObject redLiquidObject;
    [SerializeField] private float riseHeight = 1.0f;
    [SerializeField] private float riseDuration = 5.0f;
    [SerializeField] private AudioSource anomalyCSoundSource;
    [SerializeField] private AudioClip anomalyCSoundClip;
    private Vector3 redLiquidStartPos;

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
    private List<GameObject> spawnedPatients = new List<GameObject>();
 
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
#if UNITY_EDITOR
    // 디버그 단축키 - A, B, C 이상현상 수동 실행
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
        Debug.Log(" 테스트: 이상현상 A 실행");
        ActivateAnomaly(AnomalyType.A);
    }

    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
        Debug.Log(" 테스트: 이상현상 B 실행");
        ActivateAnomaly(AnomalyType.B);
    }

    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
        Debug.Log(" 테스트: 이상현상 C 실행");
        ActivateAnomaly(AnomalyType.C);
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
        Debug.Log(" 테스트: 이상현상 리셋");
        StopAllAnomalies();
    }
#endif
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
                ShowDialogue("Look around carefully. The air feels… heavy.");
                break;

            case AnomalyType.A:
                Debug.Log("이상현상 A 활성화");
                // A 타입: 모니터 전환, 조명 깜빡임, 이상한 사운드
                ShowDialogue("Something’s wrong… Get closer to the patient.");
                anomalyCoroutine = StartCoroutine(PlayAnomalyTypeA());
                break;

            case AnomalyType.B:
                Debug.Log("이상현상 B 활성화");
                // TODO: B 타입 이상현상 연출
                anomalyCoroutine = StartCoroutine(PlayAnomalyTypeB());
                break;

            case AnomalyType.C:
                Debug.Log("이상현상 C 활성화");
                anomalyCoroutine = StartCoroutine(PlayAnomalyTypeC());
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
        if (anomalyASoundSource != null)
        {
            anomalyASoundSource.Stop();
        }

        // 복제된 환자 제거
        if (spawnedPatients != null)
        {
            foreach (GameObject patient in spawnedPatients)
            {
                if (patient != null)
                {
                    Destroy(patient);
                }
            }
            spawnedPatients.Clear();
        }

        // 숨 소리 중지
        if (breathingSource != null)
        {
            breathingSource.Stop();
        }

        // 물 내려감
        if (redLiquidObject != null)
        {
            redLiquidObject.transform.position = redLiquidStartPos; // 위치 리셋
            redLiquidObject.GetComponent<MeshRenderer>().enabled = false; // 완전 안 보이게
        }

        // 물 소리 중지
        if (anomalyCSoundSource != null)
        {
            anomalyCSoundSource.Stop();
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

        if (anomalyASoundSource != null && anomalyASoundClip != null)
        {
            anomalyASoundSource.clip = anomalyASoundClip;
            anomalyASoundSource.loop = true;
            anomalyASoundSource.Play();
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

    // 이상현상 타입 B: 환자복제
    private IEnumerator PlayAnomalyTypeB()
    {
        anomalyActive = true;
        anomalyTimer = anomalyTimeLimit;

        int spawnCount = 0;
        Debug.Log("🧍‍♂️ 이상현상 B 시작 - 환자가 일정 시간마다 복제됩니다.");

        if (breathingSource != null && breathingClip != null)
        {
            breathingSource.clip = breathingClip;
            breathingSource.loop = true;
            breathingSource.Play();
            Debug.Log("🎧 breathingSource 숨소리 재생됨");
        }

        while (anomalyActive && spawnCount < maxPatientCount)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);

            GameObject clone = Instantiate(
                patientPrefab,
                spawnPoints[randomIndex].position,
                spawnPoints[randomIndex].rotation
            );

            spawnedPatients.Add(clone); // ✅ 리스트에 추가!

            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
        Debug.Log("숨소리 재생");

        Debug.Log("🧍‍♂️ 이상현상 B 완료 - 최대 환자 수에 도달했습니다.");

        yield break;

    }

    // 이상현상 타입 C: 물 차오름
    private IEnumerator PlayAnomalyTypeC()
    {
        anomalyActive = true;
        anomalyTimer = anomalyTimeLimit;

        Debug.Log("🩸 이상현상 C 시작 - 5초 후 붉은 액체 차오름");

        // 시작 위치 저장 + 렌더러 숨기기
        redLiquidStartPos = redLiquidObject.transform.position;
        redLiquidObject.transform.position = redLiquidStartPos;
        redLiquidObject.GetComponent<MeshRenderer>().enabled = false; // 🔴 숨기기

        // 5초 대기 (렌더러 OFF 상태 유지)
        yield return new WaitForSeconds(5f);

        // 이제 보여주고 상승 시작
        Debug.Log("🩸 붉은 액체 상승 시작");
        redLiquidObject.GetComponent<MeshRenderer>().enabled = true;

        // 물 소리
        if (anomalyCSoundSource != null && anomalyCSoundClip != null)
        {
            anomalyCSoundSource.clip = anomalyCSoundClip;
            anomalyCSoundSource.loop = true;
            anomalyCSoundSource.Play();
            Debug.Log("🎧 anomalyCSoundSource 테스트 재생됨");

        }

        Vector3 endPos = redLiquidStartPos + Vector3.up * riseHeight;
        float timer = 0f;

        while (timer < riseDuration)
        {
            redLiquidObject.transform.position = Vector3.Lerp(redLiquidStartPos, endPos, timer / riseDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        redLiquidObject.transform.position = endPos;
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

    // UI 표시
    private void ShowDialogue(string message)
    {
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);
        dialogueText.text = "";
        dialogueCoroutine = StartCoroutine(ShowDialogueRoutine(message));
    }

    private IEnumerator ShowDialogueRoutine(string message)
    {
        dialogueCanvasGroup.gameObject.SetActive(true);
        dialogueCanvasGroup.alpha = 0f;

        // Fade In
        float t = 0f;
        while (t < dialogueFadeDuration)
        {
            t += Time.deltaTime;
            dialogueCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / dialogueFadeDuration);
            yield return null;
        }
        dialogueCanvasGroup.alpha = 1f;

        // Typewriter 효과 실행
        yield return typewriterEffect.PlayTyping(dialogueText, message);

        // 대사 유지
        yield return new WaitForSeconds(dialogueShowDuration);

        // Fade Out
        t = 0f;
        while (t < dialogueFadeDuration)
        {
            t += Time.deltaTime;
            dialogueCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / dialogueFadeDuration);
            yield return null;
        }
        dialogueCanvasGroup.alpha = 0f;
        dialogueCanvasGroup.gameObject.SetActive(false);
    }
}