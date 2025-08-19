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
    public static UIManager Instance { get; private set; } = null;

    public SettingManager Setting => setting;
    public MainUIController MainUI { get; private set; }

    public InGameUIController InGameUI { get; private set; }

    public Stack<GameObject> OpenPanel => _openPanel;

    public SceneID CurrentScene
    {
        set
        {
            if (value == SceneID.Title)
            {
                MainUI = GameObject.FindWithTag("MainUI").GetComponent<MainUIController>();
                InGameUI = null;
            }
            else if (value == SceneID.InGame)
            {
                MainUI = null;
                InGameUI = GameObject.FindWithTag("InGameUI").GetComponent<InGameUIController>();
            }
        }
    }

    public bool AnyPanelActivated => _openPanel.Count > 0;
    #endregion

    #region ==========Fields==========

    [SerializeField] private SettingManager setting;
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Image loadingFill;

    private Stack<GameObject> _openPanel;
    #endregion

    #region ==========Unity==========
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

        _openPanel = new Stack<GameObject>();
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
            else if (InGameUI != null) InGameUI.SetActive_PausePanel(true);
        }
    }
    #endregion

    #region ==========Methods==========
    public void ClosePanel()
    {
        if (!AnyPanelActivated) return;

        _openPanel.Pop().SetActive(false);
        SoundManager.Instance.Play_UI_SFX(UISFXID.PanelClose);

        if (_openPanel.Count == 0) Time.timeScale = 1;
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

        yield return new WaitForSeconds(0.2f);

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
