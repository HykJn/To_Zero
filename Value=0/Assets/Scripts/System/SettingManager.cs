using System;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    #region =====Properties=====

    public static SettingManager Instance { get; private set; }

    #endregion

    #region =====Fields=====

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        LoadVolume();
    }

    private void OnApplicationQuit()
    {
        SaveAllSettings();
    }

    #endregion

    #region =====Methods=====

    public void SaveAllSettings()
    {
        SaveResolution();
        SaveWindowMode();
        SaveVolume();
    }

    public void SaveResolution()
    {
    }

    public void SaveWindowMode()
    {
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", SoundManager.Instance.MasterVolume);
        PlayerPrefs.SetFloat("BGMVolume", SoundManager.Instance.BGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", SoundManager.Instance.SFXVolume);
        PlayerPrefs.SetFloat("UIVolume", SoundManager.Instance.UIVolume);
        PlayerPrefs.Save();
    }

    private void LoadResolution()
    {
        
    }

    private void LoadWindowMode()
    {
        
    }

    private void LoadVolume()
    {
        SoundManager.Instance.MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        SoundManager.Instance.BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        SoundManager.Instance.SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        SoundManager.Instance.UIVolume = PlayerPrefs.GetFloat("UIVolume", 1.0f);
    }

    #endregion
}