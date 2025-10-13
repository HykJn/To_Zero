using TMPro;
using UnityEngine;

public class OptionUI : MonoBehaviour, IPanel
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private TMP_Text text_Master;
    [SerializeField] private TMP_Text text_BGM;
    [SerializeField] private TMP_Text text_SFX;
    [SerializeField] private TMP_Text text_UI;

    #endregion

    #region =====Unity Events=====

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

    public void OnValueChange_Master(float value)
    {
        SoundManager.Instance.MasterVolume = value;
        text_Master.text = ((int)(value * 100)) + "%";
    }

    public void OnValueChange_BGM(float value)
    {
        SoundManager.Instance.BGMVolume = value;
        text_BGM.text = ((int)(value * 100)) + "%";
    }

    public void OnValueChange_SFX(float value)
    {
        SoundManager.Instance.SFXVolume = value;
        text_SFX.text = ((int)(value * 100)) + "%";
    }

    public void OnValueChange_UI(float value)
    {
        SoundManager.Instance.UIVolume = value;
        text_UI.text = ((int)(value * 100)) + "%";
    }

    #endregion
}