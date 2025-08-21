using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GlobalDefines;

public class SettingPanel : Panel
{
    public Slider BGMSlider => bgmSlider;
    public Slider SFXSlider => sfxSlider;
    public Slider UISlider => uiSlider;

    [SerializeField] private GameObject closeButton;

    public TextMeshProUGUI fullScreenText;
    public TextMeshProUGUI resolutionText; // 해상도 표시 텍스트

    // --- Sound Sliders ---
    // Inspector에서 슬라이더를 연결해주세요.
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;


    private int _resolutionIndex;
    private readonly List<Vector2Int> _resolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };

    private void Start()
    {
        UpdateScreenModeText();
        UpdateResolutionText();
    }

    private void Update()
    {
        UpdateScreenModeText();
        UpdateResolutionText();
    }

    public void OnClickCloseButton()
    {
        ClosePanel();
    }

    // 전체 화면/창 모드 전환 (좌우 화살표)
    public void OnClickFullScreenLeftArrow()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenModeText();

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void OnClickFullScreenRightArrow()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenModeText();

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    // 해상도 변경 (좌우 화살표)
    public void OnClickResolutionLeftArrow()
    {
        _resolutionIndex--;
        if (_resolutionIndex < 0)
        {
            _resolutionIndex = _resolutions.Count - 1;
        }

        SetResolution();
        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void OnClickResolutionRightArrow()
    {
        _resolutionIndex++;
        if (_resolutionIndex >= _resolutions.Count)
        {
            _resolutionIndex = 0;
        }


        SetResolution();
        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    private void SetResolution()
    {
        Vector2Int resolution = _resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
        UpdateResolutionText();
    }

    private void UpdateScreenModeText()
    {
        fullScreenText.text = Screen.fullScreen ? "전체화면" : "창 모드";
    }

    private void UpdateResolutionText()
    {
        resolutionText.text = Screen.fullScreen
            ? Screen.currentResolution.width + " x " + Screen.currentResolution.height
            : Screen.width + " x " + Screen.height;
    }

    // --- 사운드 조절 함수 ---
    public void OnMasterVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        masterSlider.GetComponentInChildren<TMP_Text>().text = value + "%";
    }

    public void OnBGMVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        bgmSlider.GetComponentInChildren<TMP_Text>().text = value + "%";
    }

    public void OnSFXVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        sfxSlider.GetComponentInChildren<TMP_Text>().text = value + "%";
    }

    public void OnUIVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        uiSlider.GetComponentInChildren<TMP_Text>().text = value + "%";
    }
}