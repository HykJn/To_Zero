using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StageTransition2 : MonoBehaviour
{
    #region ==========Fields==========
    public static StageTransition2 Instance { get; private set; }

    public Volume globalVolume;
    public GameObject[] stages;

    int _currentStage = 0;
    public int CurrentStage
    {
        get => _currentStage;
        set
        {
            if (_currentStage == value) return;
            if (_isTransitioning) return;
            _currentStage = value;
            StartCoroutine(ChangeStage());
        }
    }

    public float glitchDuration = 0.3f;
    public float maxChroma = 2f;
    public float maxLens = 10f;
    public float maxGrain = 1f;

    ChromaticAberration _chroma;
    LensDistortion _lens;
    FilmGrain _grain;
    bool _isTransitioning = false;
    #endregion

    #region ==========Unity Methods==========
    void Awake()
    {
        Instance = this;

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        VolumeProfile profile = globalVolume.profile;
        profile.TryGet<ChromaticAberration>(out _chroma);
        profile.TryGet<LensDistortion>(out _lens);
        profile.TryGet<FilmGrain>(out _grain);

        _chroma.intensity.value = 0f;
        _lens.intensity.value = 0f;
        _grain.intensity.value = 0f;

    }
    #endregion

    #region ==========Methods==========
    IEnumerator ChangeStage()
    {
        _isTransitioning = true;
        float elapsed = 0f;

 
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

        //스테이지 활성화
        for (int i = 0; i < stages.Length; i++)
            stages[i].SetActive(i == _currentStage);

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
    }
    #endregion
}
