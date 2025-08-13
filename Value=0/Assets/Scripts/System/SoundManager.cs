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
    public bool TraverseBGM
    {
        get => traverseBgm;
        set
        {
            traverseBgm = value;
            bgmChannel.loop = !value;
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

    private bool traverseBgm = true;
    private int bgmIdx = 0;
    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        U_TraverseBGM();
    }
    #endregion

    #region ==========Methods==========
    public void Play_BGM(BGMID id, bool isLoop = false)
    {
        bgmIdx = (int)id;
        bgmChannel.clip = bgmClips[(int)id];
        bgmChannel.loop = isLoop;
        bgmChannel.Play();
    }

    public void Stop_BGM()
    {
        bgmChannel.Stop();
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

    private void U_TraverseBGM()
    {
        if (!TraverseBGM) return;

        float t = bgmChannel.clip.length - bgmChannel.time;

        //Fade-in/out
        if (t <= 3f)
        {
            bgmChannel.volume = masterVolume * bgmVolume * Mathf.Clamp01(t / 3f);
        }
        else if (bgmChannel.time <= 3f)
        {
            bgmChannel.volume = masterVolume * bgmVolume * Mathf.Clamp01(bgmChannel.time / 3f);
        }

        //Next track
        if (!bgmChannel.isPlaying)
        {
            bgmIdx = (bgmIdx + 1) % bgmClips.Length;
            bgmChannel.clip = bgmClips[bgmIdx];
        }
    }
    #endregion
}
