using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    public Slider BGMSlider => bgmSlider;
    public Slider SFXSlider => sfxSlider;
    public Slider UISlider => uiSlider;

    [SerializeField] private GameObject SettingPanel;

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
        new Vector2Int(2560, 1080)
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScreenModeText();
        UpdateResolutionText();

        // 슬라이더 이벤트에 함수 연결
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (uiSlider != null)
            uiSlider.onValueChanged.AddListener(OnUIVolumeChanged);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    SettingPanel.SetActive(!SettingPanel.activeSelf);

        //    GameObject player = GameObject.FindWithTag("Player");
        //    if (player == null) return;
        //    player.GetComponent<Player>().Controllable = !SettingPanel.activeSelf;
        //}
    }

    public void SetActiveSettingPanel(bool isActive)
    {
        SettingPanel.SetActive(isActive);
    }

    // 전체 화면/창 모드 전환 (좌우 화살표)
    public void OnClickFullScreenLeftArrow()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenModeText();
    }

    public void OnClickFullScreenRightArrow()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateScreenModeText();
    }

    // 해상도 변경 (좌우 화살표)
    public void OnClickResolutionLeftArrow()
    {
        resolutionIndex--;
        if (resolutionIndex < 0)
        {
            resolutionIndex = resolutions.Count - 1;
        }
        SetResolution();
    }

    public void OnClickResolutionRightArrow()
    {
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
            fullScreenText.text = "WindowMode";
        }
        else
        {
            fullScreenText.text = "FullScreen";
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
        // SoundManager의 BGMVolume 프로퍼티에 슬라이더 값을 전달
        //SoundManager.Instance.BGMVolume = value;
        //Debug.Log("BGM Volume: " + value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        sfxSlider.GetComponentInChildren<TMP_Text>().text = value.ToString() + "%";
        // SoundManager의 SFXVolume 프로퍼티에 슬라이더 값을 전달
        //SoundManager.Instance.SFXVolume = value;
        //Debug.Log("SFX Volume: " + value);
    }

    public void OnUIVolumeChanged(float value)
    {
        //Update text
        value = (int)(value * 100);
        uiSlider.GetComponentInChildren<TMP_Text>().text = value.ToString() + "%";
        // SoundManager의 UIVolume 프로퍼티에 슬라이더 값을 전달
        //SoundManager.Instance.UIVolume = value;
        //Debug.Log("UI Volume: " + value);
    }
}