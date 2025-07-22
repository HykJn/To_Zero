using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StageTransition : MonoBehaviour
{
    [Header("Post-Process Volume (Lens Distortion)")]
    public Volume globalVolume;

    [Header("전환할 스테이지")]
    public GameObject stage1;
    public GameObject stage2;

    [Header("핀치 설정 (속도↑)")]
    public float pinchDuration = 0.7f;  
    public float holdDuration = 0.4f;  
    public float unpinchDuration = 0.7f;  
    [Tooltip("축소할 때 목표 Scale 값")]
    public float pinchScale = 4f;
    [Tooltip("왜곡 강도 (음수일수록 중앙으로 더 땡겨짐)")]
    public float pinchIntensity = -0.5f;
    [Tooltip("페이드 아웃")]
    public float fadeoutColorAdj = -4f;
  

    LensDistortion _lens;
    ColorAdjustments _ColorAdj;
    bool _isTransitioning = false;
    bool _isStage1Active = true;

    void Awake()
    {
        var profile = globalVolume.profile;
        profile.TryGet<LensDistortion>(out _lens);
        profile.TryGet<ColorAdjustments>(out _ColorAdj);

        _lens.scale.value = 1f;
        _lens.intensity.value = 0f;
        _ColorAdj.postExposure.value = 0f;

        stage1.SetActive(true);
        stage2.SetActive(false);
    }

    void Update()
    {
        if (!_isTransitioning && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(DoWarpTransition());
    }

    IEnumerator DoWarpTransition()
    {
        _isTransitioning = true;

        float startScale = _lens.scale.value;
        float startLensIntensity = _lens.intensity.value;
        float elapsed = 0f;

        // 1) 빠른 축소
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
 

        // 2) 짧은 홀드
        yield return new WaitForSeconds(holdDuration);

        // 3) 스테이지 전환
        _isStage1Active = !_isStage1Active;
        stage1.SetActive(_isStage1Active);
        stage2.SetActive(!_isStage1Active);

        // 4) 빠른 확대
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
}

