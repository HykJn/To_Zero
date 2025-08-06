using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPTransition1 : MonoBehaviour
{
    public Vector3 Portal { get; set; }

    [Header("Test Portal Position")]
    [SerializeField] private Vector3 testPortalPosition = new Vector3(0, 0, 10);
    [SerializeField] private bool useTestPosition = false;

    #region ==========Fields==========
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Camera mainCamera;

    [Header("Next Stage")]
    public float pinchDuration = 0.7f;
    public float holdDuration = 0.4f;
    public float unpinchDuration = 0.7f;

    public float pinchScale = 4f;
    public float pinchIntensity = -0.5f;
    public float fadeoutColorAdj = -10f;

    LensDistortion _lens;
    ColorAdjustments _ColorAdj;
    bool _isTransitioning = false;

    // 카메라 이동 관련
    private Vector3 _originalCameraPos;
    private Quaternion _originalCameraRot;
    private float _originalCameraSize; // Orthographic Size 저장

    [Header("Camera Movement")]
    [SerializeField] private bool enableCameraMovement = true;
    [SerializeField] private float targetCameraSize = 7f; // 목표 카메라 사이즈

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Die")]
    public float glitchDuration = 0.7f;
    public float maxChroma = 2f;
    public float maxLens = 0.5f;
    public float maxGrain = 2f;

    ChromaticAberration _chroma;
    FilmGrain _grain;
    #endregion

    #region ==========Unity Methods==========
    void Awake()
    {
        VolumeProfile profile = globalVolume.profile;
        profile.TryGet<LensDistortion>(out _lens);
        profile.TryGet<ColorAdjustments>(out _ColorAdj);
        profile.TryGet<ChromaticAberration>(out _chroma);
        profile.TryGet<FilmGrain>(out _grain);

        if (mainCamera == null)
            mainCamera = Camera.main;

        // 원본 카메라 위치와 회전 저장
        if (mainCamera != null)
        {
            _originalCameraPos = mainCamera.transform.position;
            _originalCameraRot = mainCamera.transform.rotation;
            _originalCameraSize = mainCamera.orthographicSize; // 원본 카메라 사이즈 저장
        }

        if (useTestPosition)
        {
            Portal = testPortalPosition;
            Debug.Log($"테스트용 Portal 위치 설정: {Portal}");
        }
    }
    #endregion

    #region ==========Methods==========
    // 카메라를 Portal 방향으로 이동/회전시키는 함수
    void MoveCameraTowardsPortal(float t)
    {
        if (!enableCameraMovement || mainCamera == null) return;

        Vector3 currentPortal = useTestPosition ? testPortalPosition : Portal;

        // Portal 방향 계산
        Vector3 directionToPortal = (currentPortal - _originalCameraPos).normalized;

        // Portal 위치까지 정확히 이동 (t=1일 때 Portal 위치에 도달)
        mainCamera.transform.position = Vector3.Lerp(_originalCameraPos, currentPortal, t);
    }

    // 카메라를 원래 위치로 복원
    void ResetCameraPosition()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = _originalCameraPos;
            mainCamera.transform.rotation = _originalCameraRot;
            mainCamera.orthographicSize = _originalCameraSize; // 원본 카메라 사이즈로 복원
        }
    }

    // 렌즈 중심점을 Portal 방향으로 이동시키는 함수
    Vector2 GetLensCenterWithTransition(float t)
    {
        Vector3 currentPortal = useTestPosition ? testPortalPosition : Portal;

        if (currentPortal == Vector3.zero)
        {
            return new Vector2(0.5f, 0.5f); // Portal이 없으면 화면 중앙
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(currentPortal);

        // Portal의 정규화된 스크린 좌표 계산
        float normalizedX = screenPos.x / mainCamera.pixelWidth;
        float normalizedY = screenPos.y / mainCamera.pixelHeight;

        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        Vector2 portalScreenPos = new Vector2(normalizedX, normalizedY);
        Vector2 screenCenter = new Vector2(0.5f, 0.5f);

        // t=0일 때 화면 중앙(0.5, 0.5), t=1일 때 Portal 위치
        Vector2 lensCenter = Vector2.Lerp(screenCenter, portalScreenPos, t);

        Debug.Log($"t={t:F2}, Portal 스크린좌표: ({portalScreenPos.x:F2}, {portalScreenPos.y:F2}), 렌즈 중심: ({lensCenter.x:F2}, {lensCenter.y:F2})");

        return lensCenter;
    }

    public void Transition(EventID id)
    {
        if (_isTransitioning) return;
        switch (id)
        {
            case EventID.NextStage: StartCoroutine(NextStage()); break;
            case EventID.PlayerDie: StartCoroutine(NextStage()); break;
        }
    }

    IEnumerator NextStage()
    {
        _isTransitioning = true;
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = false;

        _lens.scale.value = 1f;
        _lens.intensity.value = 0f;
        _ColorAdj.postExposure.value = 0f;

        float startScale = _lens.scale.value;
        float startLensIntensity = _lens.intensity.value;
        float elapsed = 0f;

        // Pinch 단계 - 렌즈 중심점이 화면 중앙에서 Portal로 이동
        while (elapsed < pinchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pinchDuration);

            // 카메라를 Portal 방향으로 이동 (옵션)
            if (enableCameraMovement)
            {
                MoveCameraTowardsPortal(t);
            }

            // 렌즈 중심점을 Portal로 부드럽게 이동
            Vector2 lensCenter = GetLensCenterWithTransition(t);
            _lens.center.value = lensCenter;

            _lens.scale.value = Mathf.Lerp(startScale, pinchScale, t);
            _lens.intensity.value = Mathf.Lerp(startLensIntensity, pinchIntensity, t);
            _ColorAdj.postExposure.value = Mathf.Lerp(0f, fadeoutColorAdj, t);

            yield return null;
        }

        // 최대 왜곡 상태 - Portal 위치에서 완전한 왜곡
        Vector2 finalLensCenter = GetLensCenterWithTransition(1f);
        _lens.center.value = finalLensCenter;
        _lens.scale.value = pinchScale;
        _lens.intensity.value = pinchIntensity;

        if (enableCameraMovement)
        {
            MoveCameraTowardsPortal(1f); // 카메라도 최대 이동
        }

        yield return new WaitForSeconds(holdDuration);
        GameManager.Instance.Stage++;

        // Unpinch 단계 - 렌즈 중심점이 Portal에서 화면 중앙으로 복귀
        elapsed = 0f;
        while (elapsed < unpinchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / unpinchDuration);
            float reverseT = 1f - t; // 역방향 값 (1에서 0으로)

            // 카메라를 원래 위치로 복귀 (옵션)
            if (enableCameraMovement)
            {
                MoveCameraTowardsPortal(reverseT);
            }

            // 렌즈 중심점을 Portal에서 화면 중앙으로 복귀
            Vector2 lensCenter = GetLensCenterWithTransition(reverseT);
            _lens.center.value = lensCenter;

            _lens.scale.value = Mathf.Lerp(pinchScale, startScale, t);
            _lens.intensity.value = Mathf.Lerp(pinchIntensity, startLensIntensity, t);
            _ColorAdj.postExposure.value = Mathf.Lerp(fadeoutColorAdj, 0, t);

            yield return null;
        }

        // 완전히 원래 상태로 복원
        _lens.center.value = new Vector2(0.5f, 0.5f); // 확실하게 화면 중앙으로
        _lens.scale.value = startScale;
        _lens.intensity.value = startLensIntensity;

        if (enableCameraMovement)
        {
            ResetCameraPosition(); // 카메라도 원래 위치로
        }

        _isTransitioning = false;
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = true;
    }

    IEnumerator Restart()
    {
        _isTransitioning = true;
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = false;
        float elapsed = 0f;

        _chroma.intensity.value = 0f;
        _lens.intensity.value = 0f;
        _grain.intensity.value = 0f;

        while (elapsed < glitchDuration)
        {
            elapsed += Time.deltaTime;
            float noise = Mathf.PerlinNoise(Time.time * 50f, 0f) * 0.5f;

            _chroma.intensity.value = Mathf.Lerp(0f, maxChroma, noise);
            _lens.intensity.value = Mathf.Lerp(0f, maxLens, noise);
            _grain.intensity.value = Mathf.Lerp(0f, maxGrain, noise);

            yield return null;
        }

        _chroma.intensity.value = maxChroma;
        _lens.intensity.value = maxLens;
        _grain.intensity.value = maxGrain;

        GameManager.Instance.Restart();

        elapsed = 0f;
        while (elapsed < glitchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / glitchDuration);
            float smooth = 1f - Mathf.SmoothStep(0f, 1f, t);

            _chroma.intensity.value = maxChroma * smooth;
            _lens.intensity.value = maxLens * smooth;
            _grain.intensity.value = maxGrain * smooth;

            yield return null;
        }

        _chroma.intensity.value = 0f;
        _lens.intensity.value = 0f;
        _grain.intensity.value = 0f;

        _isTransitioning = false;
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = true;
    }
    #endregion
}