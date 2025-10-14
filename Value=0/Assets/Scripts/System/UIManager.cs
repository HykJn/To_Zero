using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GLOBAL;

public class UIManager : MonoBehaviour
{
    #region =====Properties=====

    public static UIManager Instance { get; private set; }

    public TitleUI TitleUI { get; set; }
    public OfficeUI OfficeUI { get; set; }
    public MatrixUI MatrixUI { get; set; }

    public OptionUI OptionPanel => optionPanel;
    public PauseUI PausePanel => pausePanel;

    #endregion

    #region =====Fields=====

    [Header("Global UI")]
    [SerializeField] private OptionUI optionPanel;
    [SerializeField] private PauseUI pausePanel;
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
            if (_openPanels.Count > 0) ClosePanel();
            else PausePanel.Open();
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
    
    public IEnumerator LoadScene(SceneID sceneID, Action beforeLoad = null, Action afterLoad = null)
    {
        AsyncOperation process = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)sceneID);
        process!.allowSceneActivation = false;

        beforeLoad?.Invoke();
        loadingPanel.SetActive(true);
        
        while (process.progress < 0.9f)
        {
            loadingBar.fillAmount = process.progress;
            yield return new WaitForEndOfFrame();
        }
        loadingBar.fillAmount = 1f;
        yield return new WaitForSeconds(0.25f);
        
        afterLoad?.Invoke();
        loadingPanel.SetActive(false);
        
        process.allowSceneActivation = true;
    }

    #endregion
}