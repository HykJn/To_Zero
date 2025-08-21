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

    public void LoadScene(SceneID scene, Action callback = null)
    {
        StartCoroutine(Co_LoadScene(scene, callback));
    }

    private IEnumerator Co_LoadScene(SceneID scene, Action callback)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene);
        operation!.allowSceneActivation = false;

        loadingPanel.OpenPanel();

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

        switch (scene)
        {
            case SceneID.Title:
                OnTitleSceneLoaded();
                break;
            case SceneID.InGame:
                OnPlaySceneLoaded();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }

        loadingPanel.ClosePanel();
        OpenPanel.Clear();

        callback?.Invoke();
    }

    private void OnTitleSceneLoaded()
    {
    }

    private void OnPlaySceneLoaded()
    {
        GameManager.Instance.Init();
    }

    #endregion
}