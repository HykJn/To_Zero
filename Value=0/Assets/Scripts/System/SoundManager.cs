using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region ==========Properties==========
    public static SoundManager Instance => instance;
    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;
            BGMVolume = BGMVolume;
            SFXVolume = SFXVolume;
            UIVolume = UIVolume;
        }
    }
    public float BGMVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = value;
            bgmChannel.volume = value * masterVolume;
        }
    }
    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = value;
            sfxChannel.volume = value * masterVolume;
        }
    }
    public float UIVolume
    {
        get => uiVolume;
        set
        {
            uiVolume = value;
            uiChannel.volume = value * masterVolume;
        }
    }
    #endregion

    #region ==========Fields==========
    private static SoundManager instance = null;

    [Header("Channels")]
    [SerializeField] private AudioSource bgmChannel;
    [SerializeField] private AudioSource sfxChannel;
    [SerializeField] private AudioSource uiChannel;

    [Header("Clips")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip[] uiClips;

    [Header("Volume")]
    [SerializeField, Range(0, 1)] private float masterVolume;
    [SerializeField, Range(0, 1)] private float bgmVolume;
    [SerializeField, Range(0, 1)] private float sfxVolume;
    [SerializeField, Range(0, 1)] private float uiVolume;
    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region ==========Methods==========
    public void Play_BGM(BGMID id, bool isLoop = false)
    {
        bgmChannel.clip = bgmClips[(int)id];
        bgmChannel.loop = isLoop;
        bgmChannel.Play();
    }

    public void Stop_BGM()
    {
        bgmChannel.Stop();

        StopCoroutine(Coroutine_BGM());
    }

    public void Play_SFX(SFXID id)
    {
        sfxChannel.clip = sfxClips[(int)id];
        sfxChannel.Play();
    }

    public void PlayOneShot_SFX(SFXID id)
    {
        sfxChannel.PlayOneShot(sfxClips[(int)id]);
    }

    public void Stop_SFX()
    {
        sfxChannel.Stop();
    }

    public void Play_UI_SFX(UISFXID id)
    {
        uiChannel.clip = uiClips[(int)id];
        uiChannel.Play();
    }

    public void PlayOneShot_UI_SFX(UISFXID id)
    {
        uiChannel.PlayOneShot(uiClips[(int)id]);
    }

    public void Stop_UI_SFX()
    {
        uiChannel.Stop();
    }

    public void TraverseBGM(BGMID startID = 0)
    {
        bgmChannel.Stop();
        bgmChannel.clip = bgmClips[(int)startID];
    }

    IEnumerator Coroutine_BGM()
    {
        throw new NotImplementedException("Coroutine_BGM is not implemented yet.");
    }
    #endregion
}
