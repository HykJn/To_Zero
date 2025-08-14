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
    public float effectDuration = 1f;
    public float pinchScale = -1.5f;
    public float pinchIntensity = -2f;
    public float pinchChorma = 3f;
    public float pinchVignette = 1f;

    bool _isTransitioning = false;

    [Header("Die")]
    public float glitchDuration = 0.7f;
    public float maxChroma = 2f;
    public float maxLens = 0.5f;
    public float maxGrain = 2f;

    LensDistortion _lens;
    ColorAdjustments _ColorAdj;
    Vignette _Vignette;
    ChromaticAberration _chroma;
    FilmGrain _grain;
    #endregion

    #region ==========Unity Methods==========
    void Awake()
    {
        VolumeProfile profile = globalVolume.profile;
        profile.TryGet<LensDistortion>(out _lens);
        profile.TryGet<ChromaticAberration>(out _chroma);
        profile.TryGet<FilmGrain>(out _grain);
        profile.TryGet<Vignette>(out _Vignette);

        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    #endregion

    #region ==========Methods==========

    public void Transition(EventID id)
    {
        if (_isTransitioning) return;
        switch (id)
        {
            case EventID.NextStage: StartCoroutine(NextStage()); break;
            case EventID.PlayerDieByDrone:
            case EventID.PlayerDieByMoves:
            case EventID.PlayerDieBySystem:
                StartCoroutine(Restart(id)); break;
        }
    }

    IEnumerator NextStage()
    {
        _isTransitioning = true;
        GameObject.FindWithTag("Player").GetComponent<Player>().Controllable = false;

        _lens.scale.value = 1f;
        _lens.intensity.value = 0f;

        float startScale = _lens.scale.value;
        float startLensIntensity = _lens.intensity.value;
        float elapsed = 0f;

        SoundManager.Instance.Play_SFX(SFXID.PortalIn);
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / effectDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _chroma.intensity.Override(Mathf.Lerp(0f, pinchChorma, smoothT));
            _lens.intensity.Override(Mathf.Lerp(0f, pinchIntensity, smoothT));
            _lens.scale.Override(Mathf.Lerp(1f, pinchScale, smoothT));
            _Vignette.intensity.Override(Mathf.Lerp(0f, pinchVignette, Mathf.InverseLerp(0.3f, 1f, t)));

            yield return null;
        }

        _chroma.intensity.Override(pinchChorma);
        _lens.scale.Override(pinchScale); _lens.intensity.Override(pinchIntensity);
        _Vignette.intensity.Override(pinchVignette);


        yield return null;
        GameManager.Instance.Stage++;

        elapsed = 0f;

        SoundManager.Instance.Play_SFX(SFXID.PortalOut);
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / effectDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _chroma.intensity.Override(Mathf.Lerp(pinchChorma, 0f, smoothT));
            _lens.intensity.Override(Mathf.Lerp(pinchIntensity, 0f, smoothT));
            _lens.scale.Override(Mathf.Lerp(pinchScale, 1f, smoothT));
            _Vignette.intensity.Override(
                          Mathf.Lerp(pinchVignette, 0f, Mathf.InverseLerp(0f, 0.8f, t)));

            yield return null;
        }

        _chroma.intensity.Override(0f);
        _lens.intensity.Override(0f); _lens.scale.Override(1f);
        _Vignette.intensity.Override(0f);

        _isTransitioning = false;
        GameObject.FindWithTag("Player").GetComponent<Player>().Controllable = true;

        GameManager.Instance.SetDialog();
    }

    IEnumerator Restart(EventID diedBy)
    {
        switch (diedBy)
        {
            case EventID.PlayerDieByDrone:
                SoundManager.Instance.Play_SFX(SFXID.DroneDetect);
                break;
            case EventID.PlayerDieByMoves:
                SoundManager.Instance.Play_SFX(SFXID.PlayerRespawn);
                break;
            case EventID.PlayerDieBySystem:
                //TODO: Implement later
                break;
        }

        _isTransitioning = true;
        GameObject.FindWithTag("Player").GetComponent<Player>().Controllable = false;
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
        GameObject.FindWithTag("Player").GetComponent<Player>().Controllable = true;
    }
    #endregion
}