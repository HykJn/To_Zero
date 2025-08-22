using UnityEngine;
using static GlobalDefines;

public class SoundManager : MonoBehaviour
{
    #region ==========Properties==========

    public static SoundManager Instance { get; private set; }

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;
            BGM_Volume = BGM_Volume;
            SFX_Volume = SFX_Volume;
            UI_Volume = UI_Volume;
        }
    }

    public float BGM_Volume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = value;
            bgmChannel.volume = value * masterVolume;
        }
    }

    public float SFX_Volume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = value;
            sfxChannel.volume = value * masterVolume;
        }
    }

    public float UI_Volume
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
        get => _traverseBgm;
        set
        {
            _traverseBgm = value;
            bgmChannel.loop = !value;
        }
    }

    #endregion

    #region ==========Fields==========

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

    private bool _traverseBgm;
    private int _bgmIdx;

    #endregion

    #region ==========Unity Methods==========

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Play_BGM(BGM_ID.Title, true);
    }

    private void Update()
    {
        U_TraverseBGM();
    }

    #endregion

    #region ==========Methods==========

    public void Play_BGM(BGM_ID id, bool isLoop = false)
    {
        _bgmIdx = (int)id;
        bgmChannel.clip = bgmClips[(int)id];
        bgmChannel.loop = isLoop;
        bgmChannel.Play();
    }

    public void Stop_BGM()
    {
        bgmChannel.Stop();
    }

    public void Play_SFX(SFX_ID id)
    {
        sfxChannel.clip = sfxClips[(int)id];
        sfxChannel.Play();
    }

    public void PlayOneShot_SFX(SFX_ID id)
    {
        sfxChannel.PlayOneShot(sfxClips[(int)id]);
    }

    public void Stop_SFX()
    {
        sfxChannel.Stop();
    }

    public void Play_UI_SFX(UI_SFX_ID id)
    {
        uiChannel.clip = uiClips[(int)id];
        uiChannel.Play();
    }

    public void PlayOneShot_UI_SFX(UI_SFX_ID id)
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
            _bgmIdx = (_bgmIdx + 1) % bgmClips.Length;
            bgmChannel.clip = bgmClips[_bgmIdx];
        }
    }

    #endregion
}