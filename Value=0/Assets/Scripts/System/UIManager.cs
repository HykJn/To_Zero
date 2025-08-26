using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GlobalDefines;

public class UIManager : MonoBehaviour
{
    #region ==========Properties==========

    public static UIManager Instance { get; private set; }
    public SettingPanel SettingPanel => settingPanel;
    public MainUIController MainUI { get; set; }
    public InGameUIController InGameUI { get; set; } = null;
    public Stack<Panel> OpenPanel { get; private set; } = new();
    public bool AnyPanelActivated => OpenPanel.Count != 0;

    #endregion

    #region ==========Fields==========

    [Header("Child Panels")]
    [SerializeField] private SettingPanel settingPanel;
    [SerializeField] private LoadingPanel loadingPanel;

    #endregion

    #region ==========Unity Events==========

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AnyPanelActivated)
            {
                OpenPanel.Pop().ClosePanel();
            }
            else
            {
                InGameUI?.PausePanel.OpenPanel();
            }
        }
    }

    #endregion

    #region ==========Methods==========

    public void ToTitle()
    {
        StartCoroutine(LoadScene(SceneID.Title));
    }

    public void ToPlay(int stage = 1)
    {
        StartCoroutine(LoadScene(SceneID.InGame, () => GameManager.Instance.Init(stage)));
    }

    private IEnumerator LoadScene(SceneID scene, Action callback = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene);
        operation!.allowSceneActivation = false;

        loadingPanel.gameObject.SetActive(true);

        float t = 0f;
        while (t <= 1.5f)
        {
            loadingPanel.LoadingFill.fillAmount = t / 1.5f;
            loadingPanel.LoadingText.text = "Now Loading" + new string('.', (int)(t * 2));

            t += Time.deltaTime;
            yield return null;
        }

        
        operation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);
        
        GameManager.Instance.Player.Controllable = false;
        callback?.Invoke();

        loadingPanel.gameObject.SetActive(false);
        OpenPanel.Clear();
    }

    #endregion
}