using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public event Action OnStageLoaded, OnRestart;

    #region =====Properties=====

    public static GameManager Instance { get; private set; }

    public Stage Stage => stages[_stageNumber];

    //보스 스테이지
    public bool CurrentBossStage => (_stageNumber+1) == bossStageNumber;

    public int StageNumber
    {
        get => _stageNumber + 1;
        set
        {
            if (value < 1 || value > stages.Length) throw new ArgumentOutOfRangeException();
            stages[_stageNumber].gameObject.SetActive(false);
            _stageNumber = value - 1;
            stages[_stageNumber].gameObject.SetActive(true);
            stages[_stageNumber].Init();
            OnStageLoaded?.Invoke();
            UIManager.Instance.MatrixUI.Stage = _stageNumber + 1;
        }
    }

    public Player Player { get; private set; }

    #endregion

    #region =====Fields=====

    [Header("Stages")]
    [SerializeField] private Stage[] stages;

    private int _stageNumber;

    //보스 스테이지
    [Header("Boss Stage")]
    [SerializeField] private int bossStageNumber = 1;

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Player>();
        StageNumber = 15;
    }

    #endregion

    #region =====Methods=====

    public void Restart()
    {
        //보스 스테이지 전용 추가
        if (CurrentBossStage)
        {
            if (Player != null)
            {
                Stage.ClearStage();
                Stage.LoadBossStage();
            }
        }
        OnRestart?.Invoke();
    }

    //보스 스테이지 값 받음
    private void OnEnable()
    {
        foreach (Stage stage in stages)
        {
            stage.OnBossStageLoaded += HandleBossStage;
        }
    }
    private void OnDisable()
    {
        foreach (Stage stage in stages)
        {
            stage.OnBossStageLoaded -= HandleBossStage;
        }
    }
    private void HandleBossStage(int startnumber, int moves, Vector3 startPos)
    {
        Player.Value = startnumber;
        Player.Moves = moves;
        Player.transform.position = startPos;

    }

    #endregion
}