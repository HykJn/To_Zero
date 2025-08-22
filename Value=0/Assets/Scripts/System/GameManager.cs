using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    #region ==========Events==========

    public event Action OnInit;
    public event Action OnStageLoad;
    public event Action OnRestart;

    #endregion

    #region ==========Properties==========

    public static GameManager Instance { get; private set; }

    public Stage Stage => stages[_curStage - 1];

    public int StageNumber
    {
        get => _curStage;
        set
        {
            _curStage = value;
            for (int i = 0; i < stages.Length; i++)
                stages[i].gameObject.SetActive(i == value - 1);

            UIManager.Instance.InGameUI.Stage = value;
            
            OnStageLoad?.Invoke();
            Restart();
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

    public void Init(int stageNumber)
    {
        foreach (Stage stage in stages)
            stage.gameObject.SetActive(false);

        _curStage = stageNumber;

        foreach (Stage stage in stages)
            stage.Init();

        OnInit?.Invoke();

        // Stage.gameObject.SetActive(true);
        StageNumber = stageNumber;
    }


    public void Restart()
    {
        Stage.Restart();

        OnRestart?.Invoke();
    }

    #endregion
}