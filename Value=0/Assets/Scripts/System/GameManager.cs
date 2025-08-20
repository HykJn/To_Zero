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

    public static GameManager Instance { get; private set; } = null;

    public Stage Stage => stages[curStage - 1];

    public int StageNumber
    {
        get => curStage;
        set
        {
            stages[curStage - 1].gameObject.SetActive(false);
            curStage = value;
            stages[curStage - 1].gameObject.SetActive(true);
            OnStageLoad?.Invoke();
        }
    }

    #endregion

    #region ==========Fields==========

    [SerializeField] private Stage[] stages;
    [SerializeField] private int curStage;
    [SerializeField] private GameObject demoCanvas;

    [SerializeField] private DialogueSystem dialogSystem;

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
    }

    public void Restart()
    {
        foreach (Stage stage in stages)
            stage.Restart();
        OnRestart?.Invoke();
    }

    #endregion
}