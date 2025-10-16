using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GLOBAL;

public class UIManager : MonoBehaviour
{
    #region =====Properties=====

    public static UIManager Instance { get; private set; }

    public TitleUI TitleUI { get; set; }
    public OfficeUI OfficeUI { get; set; }
    public MatrixUI MatrixUI { get; set; }

    public OptionPanel OptionPanel => optionPanel;
    public PausePanel PausePanel => pausePanel;
    public DialogPanel DialogPanel => dialogPanel;
    public bool AnyPanelOpen => _openPanels.Count > 0;

    #endregion

    #region =====Fields=====

    [Header("Global UI")]
    [SerializeField] private OptionPanel optionPanel;
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private DialogPanel dialogPanel;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;

    private Stack<IPanel> _openPanels = new();

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_openPanels.Count > 0 && !_openPanels.Peek().Equals(dialogPanel)) ClosePanel();
            else if (SequanceManager.SceneID != SceneID.Title) PausePanel.Open();
        }
    }

    #endregion

    #region =====Methods=====

    public void OpenPanel(IPanel panel) => _openPanels.Push(panel);

    public void ClosePanel(IPanel panel = null)
    {
        if (_openPanels.Count == 0) return;
        if (panel == null || _openPanels.Peek() == panel) _openPanels.Pop().Close();
        else panel.ForceClose();
    }

    public void CloseAllPanels()
    {
        while (_openPanels.Count > 0) _openPanels.Pop().Close();
    }

    public void LoadScene(SceneID sceneID, Action beforeLoad = null, Action afterLoad = null) =>
        StartCoroutine(Crtn_LoadScene(sceneID, beforeLoad, afterLoad));

    private IEnumerator Crtn_LoadScene(SceneID sceneID, Action beforeLoad = null, Action afterLoad = null)
    {
        AsyncOperation process = SceneManager.LoadSceneAsync((int)sceneID);
        process!.allowSceneActivation = false;

        beforeLoad?.Invoke();
        loadingPanel.SetActive(true);

        while (process.progress < 0.9f)
        {
            loadingBar.fillAmount = process.progress;
            yield return new WaitForEndOfFrame();
        }

        loadingBar.fillAmount = 1f;
        process.allowSceneActivation = true;
        while (!process.isDone) yield return new WaitForEndOfFrame();

        afterLoad?.Invoke();

        loadingPanel.SetActive(false);

        SequanceManager.SceneID = sceneID;
    }

    #endregion
}