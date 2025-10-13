using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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

    [Header("Global Canvas")]
    [SerializeField] private OptionUI optionPanel;
    [SerializeField] private PauseUI pausePanel;

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

    #endregion
}