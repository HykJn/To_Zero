using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StageTransition2 : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public Volume globalVolume;

    [Header("전환할 스테이지")]
    public GameObject stage1;
    public GameObject stage2;

    [Header("Glitch 설정")]
    public float glitchDuration = 0.3f;
    public float maxChroma = 2f;
    public float maxLens = 10f;
    public float maxGrain = 1f;

    ChromaticAberration _chroma;
    LensDistortion _lens;
    FilmGrain _grain;
    bool _isTransitioning = false;
    bool _isStage1Active = true;

    void Awake()
    {
        // Volume Profile에서 효과 인스턴스 꺼내기
        var profile = globalVolume.profile;
        profile.TryGet<ChromaticAberration>(out _chroma);
        profile.TryGet<LensDistortion>(out _lens);
        profile.TryGet<FilmGrain>(out _grain);

        // 초기 상태 보장
        _chroma.intensity.value = 0f;
        _lens.intensity.value = 0f;
        _grain.intensity.value = 0f;

        // Stage1만 켜고 시작
        stage1.SetActive(true);
        stage2.SetActive(false);
    }

    void Update()
    {
        if (!_isTransitioning && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(DoGlitchSwap());
    }

    IEnumerator DoGlitchSwap()
    {
        _isTransitioning = true;
        float elapsed = 0f;

        // 1) Glitch 인텐시티 ↑ (0 → max)
        while (elapsed < glitchDuration)
        {
            elapsed += Time.deltaTime;
            float noise = Mathf.PerlinNoise(Time.time * 50f, 0f) * 0.5f;

            _chroma.intensity.value = Mathf.Lerp(0f, maxChroma, noise);
            _lens.intensity.value = Mathf.Lerp(0f, maxLens, noise);
            _grain.intensity.value = Mathf.Lerp(0f, maxGrain, noise);

            yield return null;
        }
        // 최대치 고정
        _chroma.intensity.value = maxChroma;
        _lens.intensity.value = maxLens;
        _grain.intensity.value = maxGrain;

        // 2) 스테이지 전환
        _isStage1Active = !_isStage1Active;
        stage1.SetActive(_isStage1Active);
        stage2.SetActive(!_isStage1Active);

        // 3) Glitch 해제 ↓ (max → 0)
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
        // 초기화
        _chroma.intensity.value = 0f;
        _lens.intensity.value = 0f;
        _grain.intensity.value = 0f;

        _isTransitioning = false;
    }
}
