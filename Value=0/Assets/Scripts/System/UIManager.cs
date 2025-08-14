using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region ==========Properties==========
    public static UIManager Instance => instance;

    public SettingManager Setting => setting;
    public MainUIController MainUI => mainUI;
    public InGameUIController InGameUI => inGameUI;

    public Stack<GameObject> OpenPanel => openPanel;

    public SceneID CurrentScene
    {
        set
        {
            if (value == SceneID.Title)
            {
                mainUI = GameObject.FindWithTag("MainUI").GetComponent<MainUIController>();
                inGameUI = null;
            }
            else if (value == SceneID.InGame)
            {
                mainUI = null;
                inGameUI = GameObject.FindWithTag("InGameUI").GetComponent<InGameUIController>();
            }
        }
    }

    public bool AnyPanelActivated => openPanel.Count > 0;
    #endregion

    #region ==========Fields==========
    private static UIManager instance = null;

    [SerializeField] private SettingManager setting;
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Image loadingFill;

    private MainUIController mainUI;
    private InGameUIController inGameUI;

    private Stack<GameObject> openPanel;
    #endregion

    #region ==========Unity==========
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

        openPanel = new Stack<GameObject>();
    }

    private void Start()
    {
        CurrentScene = SceneID.Title;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AnyPanelActivated) ClosePanel();
            else if (inGameUI != null) inGameUI.SetActive_PausePanel(true);
        }
    }
    #endregion

    #region ==========Methods==========
    public void ClosePanel()
    {
        if (!AnyPanelActivated) return;

        openPanel.Pop().SetActive(false);
        SoundManager.Instance.Play_UI_SFX(UISFXID.PanelClose);

        if (openPanel.Count == 0) Time.timeScale = 1;
    }

    private IEnumerator LoadScene(SceneID sceneID, Action onSceneLoaded = null, Action onSceneChanged = null)
    {
        SoundManager.Instance.Stop_BGM();
        AsyncOperation loading = SceneManager.LoadSceneAsync((int)sceneID);
        loading.allowSceneActivation = false;
        loadingPanel.blocksRaycasts = true;
        float t = 0f;
        loadingText.text = "Now Loading";

        loadingPanel.alpha = 1f;
        while (t <= 3)
        {
            t += Time.deltaTime;
            //loadingPanel.alpha = Mathf.Clamp01(t);
            loadingFill.fillAmount = t / 3f;
            loadingText.text = "Now Loading" + new string('.', (int)(t * 3) % 6);
            yield return null;
        }

        loading.allowSceneActivation = true;

        loadingPanel.alpha = 1;
        loadingText.text = "Press any key to continue...";

        while (!Input.anyKeyDown) yield return null;
        CurrentScene = sceneID;

        BGMID id = sceneID switch
        {
            SceneID.Title => BGMID.Title,
            SceneID.InGame => BGMID.InGame,
            _ => throw new InvalidOperationException()
        };
        SoundManager.Instance.Play_BGM(id, true);

        onSceneLoaded?.Invoke();

        //t = 1;
        //while (t >= 0)
        //{
        //    t -= Time.deltaTime;
        //    loadingPanel.alpha = Mathf.Clamp01(t);
        //    yield return null;
        //}

        loadingPanel.alpha = 0;
        loadingPanel.blocksRaycasts = false;
        onSceneChanged?.Invoke();
    }

    public void ToPlay(int stage)
    {
        void OnSceneLoaded()
        {
            GameManager.Instance.Stage = stage;
            GameManager.Instance.SetDialog();
            GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = false;
        }

        void OnSceneChanged()
        {
            GameObject.FindWithTag("Player").GetComponent<Player>().IsMovable = true;
        }

        StartCoroutine(LoadScene(SceneID.InGame, OnSceneLoaded, OnSceneChanged));
    }

    public void ToTitle()
    {
        StartCoroutine(LoadScene(SceneID.Title));
    }
    #endregion
}
