using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPTransition : MonoBehaviour
{
    public Vector3 Portal { get; set; }

    #region ==========Fields==========
    [SerializeField] private Volume globalVolume;

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

    [Header("Die")]
    public float glitchDuration = 0.7f;
    public float maxChroma = 2f;
    public float maxLens = 0.5f;
    public float maxGrain = 2f;

    ChromaticAberration _chroma;
    //LensDistortion _lens;
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
    }
    #endregion

    #region ==========Methods==========
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
            _lens.scale.value = Mathf.Lerp(startScale, pinchScale, t);
            _lens.intensity.value = Mathf.Lerp(startLensIntensity, pinchIntensity, t);
            _ColorAdj.postExposure.value = Mathf.Lerp(0f, fadeoutColorAdj, t);
            //this.transform.position += (Portal - this.transform.position).normalized * elapsed;
            yield return null;
        }
        //this.transform.position = Portal;
        _lens.scale.value = pinchScale;
        _lens.intensity.value = pinchIntensity;


        //����ߴٰ� ��������Ȱ��ȭ
        yield return new WaitForSeconds(holdDuration);
        GameManager.Instance.Stage++;


        elapsed = 0f;
        while (elapsed < unpinchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / unpinchDuration);
            _lens.scale.value = Mathf.Lerp(pinchScale, startScale, t);
            _lens.intensity.value = Mathf.Lerp(pinchIntensity, startLensIntensity, t);
            _ColorAdj.postExposure.value = Mathf.Lerp(fadeoutColorAdj, 0, t);
            //this.transform.position += (Vector3.zero - this.transform.position).normalized * elapsed;

            yield return null;
        }
        _lens.scale.value = startScale;
        _lens.intensity.value = startLensIntensity;


        _isTransitioning = false;
        GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = true;
        //this.transform.position = Vector3.back;
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

        //�������� Ȱ��ȭ
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

