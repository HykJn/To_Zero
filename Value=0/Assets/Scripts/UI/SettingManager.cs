using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public Slider BGMSlider => bgmSlider;
    public Slider SFXSlider => sfxSlider;
    public Slider UISlider => uiSlider;

    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject closeButton;

    public TextMeshProUGUI fullScreenText;
    public TextMeshProUGUI resolutionText; // 해상도 표시 텍스트

    // --- Sound Sliders ---
    // Inspector에서 슬라이더를 연결해주세요.
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;


    private int resolutionIndex = 0;
    private List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };

    private void Start()
    {
        UpdateScreenModeText();
        UpdateResolutionText();
    }

    public void SetActiveSettingPanel(bool isActive)
    {
        SoundManager.Instance.Play_UI_SFX(isActive ? UISFXID.PanelOpen : UISFXID.PanelClose);
        Time.timeScale = isActive ? 0 : 1;
        SettingPanel.SetActive(isActive);

        if (isActive) UIManager.Instance.OpenPanel.Push(SettingPanel);
    }

    public void OnClickCloseButton()
    {
        //SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        //SetActiveSettingPanel(false);

        UIManager.Instance.ClosePanel();
        Time.timeScale = 1;
    }

    // 전체 화면/창 모드 전환 (좌우 화살표)
    public void OnClickFullScreenLeftArrow()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenModeText();
    }

    public void OnClickFullScreenRightArrow()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenModeText();
    }

    // 해상도 변경 (좌우 화살표)
    public void OnClickResolutionLeftArrow()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        resolutionIndex--;
        if (resolutionIndex < 0)
        {
            resolutionIndex = resolutions.Count - 1;
        }
        SetResolution();
    }

    public void OnClickResolutionRightArrow()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        resolutionIndex++;
        if (resolutionIndex >= resolutions.Count)
        {
            resolutionIndex = 0;
        }
        SetResolution();
    }

    private void SetResolution()
    {
        Vector2Int resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
        UpdateResolutionText();
    }

    private void UpdateScreenModeText()
    {
        if (Screen.fullScreen)
        {
            fullScreenText.text = "Full Screen";
        }
        else
        {
            fullScreenText.text = "Window mode";
        }
    }

    private void UpdateResolutionText()
    {
        Vector2Int resolution = resolutions[resolutionIndex];
        resolutionText.text = resolution.x + " x " + resolution.y;
    }

    // --- 사운드 조절 함수 ---
    public void OnMasterVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        masterSlider.GetComponentInChildren<TMP_Text>().text = value.ToString() + "%";
    }

    public void OnBGMVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        bgmSlider.GetComponentInChildren<TMP_Text>().text = value.ToString() + "%";
    }

    public void OnSFXVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        sfxSlider.GetComponentInChildren<TMP_Text>().text = value.ToString() + "%";
    }

    public void OnUIVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        uiSlider.GetComponentInChildren<TMP_Text>().text = value.ToString() + "%";
    }
}