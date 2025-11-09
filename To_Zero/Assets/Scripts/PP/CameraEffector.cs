using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static GLOBAL;

public class CameraEffector : MonoBehaviour
{
    #region ==========Properties==========

    public static CameraEffector Instance { get; private set; }

    #endregion

    #region ==========Fields==========

    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Camera mainCamera;

    [Header("Next Stage")]
    public float effectDuration = 1f;
    public float pinchScale = -1.5f;
    public float pinchIntensity = -2f;
    public float pinchChroma = 3f;
    public float pinchVignette = 1f;

    [Header("Die")]
    public float glitchDuration = 0.7f;
    public float maxChroma = 2f;
    public float maxLens = 0.5f;
    public float maxGrain = 2f;

    private bool _isTransitioning;
    private LensDistortion _lens;
    private ColorAdjustments _colorAdj;
    private Vignette _vignette;
    private ChromaticAberration _chroma;
    private FilmGrain _grain;

    #endregion

    #region ==========Unity Methods==========

    private void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;

        VolumeProfile profile = globalVolume.profile;
        profile.TryGet(out _lens);
        profile.TryGet(out _chroma);
        profile.TryGet(out _grain);
        profile.TryGet(out _vignette);

        mainCamera ??= Camera.main;
    }

    #endregion

    #region ==========Methods==========

    // public void Transition(EventID id)
    // {
    //     if (_isTransitioning) return;
    //     switch (id)
    //     {
    //         case EventID.NextStage: StartCoroutine(NextStage()); break;
    //         case EventID.PlayerDie:
    //             StartCoroutine(Restart(id)); break;
    //         default:
    //             return;
    //     }
    // }

    public void NextStage() => StartCoroutine(Crtn_NextStage());
    public void Restart() => StartCoroutine(Crtn_Restart());

    private IEnumerator Crtn_NextStage()
    {
        _isTransitioning = true;
        GameManager.Instance.Player.IsMovable = false;

        _lens.scale.value = 1f;
        _lens.intensity.value = 0f;

        float elapsed = 0f;

        SoundManager.Instance.Play(SFX_ID.EnterPortal);
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / effectDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _chroma.intensity.Override(Mathf.Lerp(0f, pinchChroma, smoothT));
            _lens.intensity.Override(Mathf.Lerp(0f, pinchIntensity, smoothT));
            _lens.scale.Override(Mathf.Lerp(1f, pinchScale, smoothT));
            _vignette.intensity.Override(Mathf.Lerp(0f, pinchVignette, Mathf.InverseLerp(0.3f, 1f, t)));

            yield return null;
        }

        _chroma.intensity.Override(pinchChroma);
        _lens.scale.Override(pinchScale);
        _lens.intensity.Override(pinchIntensity);
        _vignette.intensity.Override(pinchVignette);


        yield return null;
        GameManager.Instance.StageNumber++;
        GameManager.Instance.Player.IsMovable = false;

        elapsed = 0f;

        SoundManager.Instance.Play(SFX_ID.OutPortal);
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / effectDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _chroma.intensity.Override(Mathf.Lerp(pinchChroma, 0f, smoothT));
            _lens.intensity.Override(Mathf.Lerp(pinchIntensity, 0f, smoothT));
            _lens.scale.Override(Mathf.Lerp(pinchScale, 1f, smoothT));
            _vignette.intensity.Override(
                Mathf.Lerp(pinchVignette, 0f, Mathf.InverseLerp(0f, 0.8f, t)));

            yield return null;
        }

        _chroma.intensity.Override(0f);
        _lens.intensity.Override(0f);
        _lens.scale.Override(1f);
        _vignette.intensity.Override(0f);

        _isTransitioning = false;
        GameManager.Instance.Player.IsMovable = true;
        GameManager.Instance.LoadDialog();
    }

    private IEnumerator Crtn_Restart()
    {
        // switch (diedBy)
        // {
        //     case EventID.PlayerDieByDrone:
        //         SoundManager.Instance.Play_SFX(SFX_ID.DroneDetect);
        //         break;
        //     case EventID.PlayerDieByMoves:
        //         SoundManager.Instance.Play_SFX(SFX_ID.PlayerRespawn);
        //         break;
        //     case EventID.PlayerDieBySystem:
        //         //TODO: Implement later
        //         break;
        // }

        _isTransitioning = true;
        GameManager.Instance.Player.IsMovable = false;
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
        GameManager.Instance.Player.IsMovable = false;

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
        GameManager.Instance.Player.IsMovable = true;
    }

    #endregion
}