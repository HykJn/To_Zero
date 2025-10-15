using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnStageLoaded, OnRestart;

    #region =====Properties=====

    public static GameManager Instance { get; private set; }

    public Stage Stage => stages[_stageNumber];

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
        OnRestart?.Invoke();
    }

    #endregion
}