using System;
using UnityEngine;
using static GLOBAL;

public class SoundManager : MonoBehaviour
{
    #region =====Properties=====

    public static SoundManager Instance { get; private set; }

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;
            bgmChannel.volume = bgmVolume * value;
            sfxChannel.volume = sfxVolume * value;
            uiChannel.volume = uiVolume * value;
        }
    }

    public float BGMVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = value;
            bgmChannel.volume = bgmVolume * value;
        }
    }

    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = value;
            sfxChannel.volume = sfxVolume * value;
        }
    }

    public float UIVolume
    {
        get => uiVolume;
        set
        {
            uiVolume = value;
            uiChannel.volume = uiVolume * value;
        }
    }

    #endregion

    #region =====Fields=====

    [SerializeField, Range(0, 1)] private float masterVolume;
    [SerializeField] private AudioSource bgmChannel;
    [SerializeField, Range(0, 1)] private float bgmVolume;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioSource sfxChannel;
    [SerializeField, Range(0, 1)] private float sfxVolume;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioSource uiChannel;
    [SerializeField, Range(0, 1)] private float uiVolume;
    [SerializeField] private AudioClip[] uiClips;

    #endregion

    #region =====Unity Event=====

    private void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    #region =====Methods=====

    public void Play(BGM_ID bgmID, LoopType loopType = LoopType.Single, TraverseType traverseType = TraverseType.None)
    {
        bgmChannel.clip = bgmClips[(int)bgmID];
        bgmChannel.loop = loopType == LoopType.Single ? true : false;

        bgmChannel.Play();
    }

    public void Play(SFX_ID sfxID)
    {
        sfxChannel.clip = sfxClips[(int)sfxID];
        sfxChannel.Play();
    }

    public void Play(UI_SFX_ID uiSfxID)
    {
        uiChannel.clip = uiClips[(int)uiSfxID];
        uiChannel.Play();
    }

    public void PlayOneShot(BGM_ID bgmID) => bgmChannel.PlayOneShot(bgmClips[(int)bgmID]);
    public void PlayOneShot(SFX_ID sfxID) => sfxChannel.PlayOneShot(sfxClips[(int)sfxID]);
    public void PlayOneShot(UI_SFX_ID uiSfxID) => uiChannel.PlayOneShot(uiClips[(int)uiSfxID]);

    public void Stop(AudioChannel channel)
    {
        AudioSource source = channel switch
        {
            AudioChannel.BGM => bgmChannel,
            AudioChannel.SFX => sfxChannel,
            AudioChannel.UI => uiChannel,
            _ => throw new ArgumentException()
        };
        source.Stop();
    }

    public void Mute(AudioChannel channel, bool mute = true)
    {
        AudioSource source = channel switch
        {
            AudioChannel.BGM => bgmChannel,
            AudioChannel.SFX => sfxChannel,
            AudioChannel.UI => uiChannel,
            _ => throw new ArgumentException()
        };
        source.mute = mute;
    }

    #endregion
}