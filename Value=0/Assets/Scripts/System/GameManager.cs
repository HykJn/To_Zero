using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    #region ==========Events==========

    public event Action OnInit;
    public event Action OnStageLoad;
    public event Action OnRestart;
    public event Action OnPlayerMove;

    #endregion

    #region ==========Properties==========

    public static GameManager Instance { get; private set; }

    public Stage Stage => stages[_curStage - 1];

    public int StageNumber
    {
        get => _curStage;
        set
        {
            stages[_curStage - 1].gameObject.SetActive(false);
            _curStage = value;
            stages[_curStage - 1].gameObject.SetActive(true);

            OnStageLoad?.Invoke();
        }
    }

    public Player Player => player;

    #endregion

    #region ==========Fields==========
    
    [Header("Stage Configuration")]
    [SerializeField] private Player player;
    [SerializeField] private Stage[] stages;
    [SerializeField] private GameObject demoCanvas;

    private int _curStage;

    #endregion

    #region ==========Unity Methods==========

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    #endregion

    #region ==========Methods==========

    public void Init()
    {
        foreach (Stage stage in stages)
            stage.Init();

        OnInit?.Invoke();

        UIManager.Instance.InGameUI.DialogPanel.SetDialog(Stage.Dialogs);
    }

    public void Restart()
    {
        foreach (Stage stage in stages)
            stage.Restart();
        OnRestart?.Invoke();
    }

    #endregion
}