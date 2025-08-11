using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPTransition : MonoBehaviour
{
    public Vector3 Portal { get; set; }


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
    [SerializeField] private float fadeStartDelay = 0.3f;

    LensDistortion _lens;
    ColorAdjustments _ColorAdj;
    bool _isTransitioning = false;

    private float _originalCameraSize; // Orthographic Size 저장
    [SerializeField] private float CameraSize = 2f;

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

        if (mainCamera != null)
        {
            _originalCameraSize = mainCamera.orthographicSize;
        }
    }
    #endregion

    #region ==========Methods==========
    void ResetCameraSize()
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = _originalCameraSize;
        }
    }

    // 렌즈 중심점을 Portal 방향으로 이동시키는 함수
    Vector2 GetLensCenterWithTransition(float t)
    {
        Vector3 currentPortal = Portal;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(currentPortal);


        float normalizedX = screenPos.x / mainCamera.pixelWidth;
        float normalizedY = screenPos.y / mainCamera.pixelHeight;

        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        Vector2 portalScreenPos = new Vector2(normalizedX, normalizedY);
        Vector2 screenCenter = new Vector2(0.5f, 0.5f);

        Vector2 lensCenter = Vector2.Lerp(screenCenter, portalScreenPos, t);
        return lensCenter;
    }

    public void Transition(EventID id)
    {
        if (_isTransitioning) return;
        switch (id)
        {
            case EventID.NextStage: StartCoroutine(NextStage()); break;
            case EventID.PlayerDie: StartCoroutine(Restart()); break;
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
        while (elapsed < pinchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pinchDuration);

            // 렌즈 중심점을 Portal로 이동
            //Vector2 lensCenter = GetLensCenterWithTransition(t);
            //_lens.center.value = lensCenter;

            mainCamera.orthographicSize = Mathf.Lerp(_originalCameraSize, CameraSize, t);
            _lens.scale.value = Mathf.Lerp(startScale, pinchScale, t);
            _lens.intensity.value = Mathf.Lerp(startLensIntensity, pinchIntensity, t);
            //_ColorAdj.postExposure.value = Mathf.Lerp(0f, fadeoutColorAdj, t);
            //0.3초부터 fadeout 진행
            float fadeT = 0f;
            if (elapsed > fadeStartDelay)
            {
                float fadeElapsed = elapsed - fadeStartDelay;
                float fadeDuration = pinchDuration - fadeStartDelay;
                fadeT = Mathf.Clamp01(fadeElapsed / fadeDuration);
            }
            _ColorAdj.postExposure.value = Mathf.Lerp(0f, fadeoutColorAdj, fadeT);
            yield return null;
        }

        Vector2 finalLensCenter = GetLensCenterWithTransition(1f);
        _lens.center.value = finalLensCenter;
        _lens.scale.value = pinchScale;
        _lens.intensity.value = pinchIntensity;

        yield return new WaitForSeconds(holdDuration);
        GameManager.Instance.Stage++;

        elapsed = 0f;
        while (elapsed < unpinchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / unpinchDuration);
            float reverseT = 1f - t; // 역방향 값 

            // 화면 중앙으로 복귀
            Vector2 lensCenter = GetLensCenterWithTransition(reverseT);
            _lens.center.value = lensCenter;

            mainCamera.orthographicSize = Mathf.Lerp(CameraSize, _originalCameraSize, t);
            _lens.scale.value = Mathf.Lerp(pinchScale, startScale, t);
            _lens.intensity.value = Mathf.Lerp(pinchIntensity, startLensIntensity, t);
            _ColorAdj.postExposure.value = Mathf.Lerp(fadeoutColorAdj, 0, t);

            yield return null;
        }

        _lens.center.value = new Vector2(0.5f, 0.5f); //화면 중앙으로
        _lens.scale.value = startScale;
        _lens.intensity.value = startLensIntensity;

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
            float lensSmooth = 1f - Mathf.SmoothStep(0f, 1f, t * 0.8f);

            _chroma.intensity.value = maxChroma * smooth;
            _lens.intensity.value = maxLens * lensSmooth;
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