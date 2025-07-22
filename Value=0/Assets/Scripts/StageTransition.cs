using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StageTransition : MonoBehaviour
{
    #region ==========Fields==========
    public static StageTransition Instance { get; private set; }

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
            StartCoroutine(StageChange());
        }
    }

 
    public float pinchDuration = 0.7f;  
    public float holdDuration = 0.4f;  
    public float unpinchDuration = 0.7f;  

    public float pinchScale = 4f;
    public float pinchIntensity = -0.5f;

    public float fadeoutColorAdj = -10f;
  
    LensDistortion _lens;
    ColorAdjustments _ColorAdj;
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
        profile.TryGet<LensDistortion>(out _lens);
        profile.TryGet<ColorAdjustments>(out _ColorAdj);

        _lens.scale.value = 1f;
        _lens.intensity.value = 0f;
        _ColorAdj.postExposure.value = 0f;
    }
    #endregion

    #region ==========Methods==========
    IEnumerator StageChange()
    {
        _isTransitioning = true;

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
            yield return null;
        }
        _lens.scale.value = pinchScale;
        _lens.intensity.value = pinchIntensity;
 

       //대기했다가 스테이지활성화
        yield return new WaitForSeconds(holdDuration);
        for(int i = 0; i < stages.Length; i++)
            stages[i].SetActive(i == _currentStage);

       
        elapsed = 0f;
        while (elapsed < unpinchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / unpinchDuration);
            _lens.scale.value = Mathf.Lerp(pinchScale, startScale, t);
            _lens.intensity.value = Mathf.Lerp(pinchIntensity, startLensIntensity, t);
            _ColorAdj.postExposure.value = Mathf.Lerp(fadeoutColorAdj, 0, t);

            yield return null;
        }
        _lens.scale.value = startScale;
        _lens.intensity.value = startLensIntensity;
        

        _isTransitioning = false;
    }
    #endregion
}

