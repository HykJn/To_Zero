using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GLOBAL;

public class OptionUI : MonoBehaviour, IPanel
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private TMP_Text text_WindowMode;
    [SerializeField] private TMP_Text text_Resolution;
    [Space]
    [SerializeField] private Slider masterVolume;
    [SerializeField] private TMP_Text text_Master;
    [SerializeField] private Slider bgmVolume;
    [SerializeField] private TMP_Text text_BGM;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private TMP_Text text_SFX;
    [SerializeField] private Slider uiVolume;
    [SerializeField] private TMP_Text text_UI;

    private int resIdx;

    #endregion

    #region =====Unity Events=====

    private void Start()
    {
        text_WindowMode.text = Screen.fullScreen ? "전체 화면" : "창 모드";

        for (int i = Screen.resolutions.Length - 1; i >= 0; i--)
        {
            if (!Equals(Screen.resolutions[i], Screen.currentResolution)) continue;
            resIdx = i;
            text_Resolution.text = Screen.resolutions[i].width + "x" + Screen.resolutions[i].height;
            break;
        }

        masterVolume.value = SoundManager.Instance.MasterVolume;
        bgmVolume.value = SoundManager.Instance.BGMVolume;
        sfxVolume.value = SoundManager.Instance.SFXVolume;
        uiVolume.value = SoundManager.Instance.UIVolume;
    }

    #endregion

    #region =====Methods=====

    public void Open()
    {
        UIManager.Instance.OpenPanel(this);
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        UIManager.Instance.ClosePanel(this);
        this.gameObject.SetActive(false);
    }

    public void ForceClose()
    {
        this.gameObject.SetActive(false);
    }

    public void OnClick_WindowMode()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);   
        Screen.fullScreen = !Screen.fullScreen;
        text_WindowMode.text = Screen.fullScreen ? "전체 화면" : "창 모드";
    }

    public void OnClick_Resolution(int direction)
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        resIdx = Mathf.Clamp(resIdx + direction, 0, Screen.resolutions.Length - 1);
        int width = Screen.resolutions[resIdx].width;
        int height = Screen.resolutions[resIdx].height;
        Screen.SetResolution(width, height, Screen.fullScreen);
        text_Resolution.text = width + " X " + height;
    }

    public void OnValueChange_Master(float value)
    {
        SoundManager.Instance.MasterVolume = value;
        text_Master.text = ((int)(value * 100)) + "%";
    }

    public void OnValueChange_BGM(float value)
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        SoundManager.Instance.BGMVolume = value;
        text_BGM.text = ((int)(value * 100)) + "%";
    }

    public void OnValueChange_SFX(float value)
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        SoundManager.Instance.SFXVolume = value;
        text_SFX.text = ((int)(value * 100)) + "%";
    }

    public void OnValueChange_UI(float value)
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        SoundManager.Instance.UIVolume = value;
        text_UI.text = ((int)(value * 100)) + "%";
    }

    #endregion
}