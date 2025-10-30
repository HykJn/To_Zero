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
            SequanceManager.Stage = value;
        }
    }

    public Player Player => player;

    #endregion

    #region =====Fields=====

    [SerializeField] private Player player;
    
    [Header("Stages")]
    [SerializeField] private Stage[] stages;

    private int _stageNumber;

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region =====Methods=====

    public void Restart()
    {
        OnRestart?.Invoke();
    }

    public void LoadDialog(int stage)
    {
        if (stage == 1)
        {
            UIManager.Instance.DialogPanel.SetDialog(10);
            UIManager.Instance.DialogPanel.StartDialog();
        }
        else if (stage == 2)
        {
            SequanceManager.Chapter = 1;
        }
        else if (stage == 4)
        {
            UIManager.Instance.DialogPanel.SetDialog(11);
            UIManager.Instance.DialogPanel.StartDialog();
        }
        else if (stage == 6)
        {
            UIManager.Instance.DialogPanel.SetDialog(12);
            UIManager.Instance.DialogPanel.StartDialog();
        }
        else if (stage == 10)
        {
            UIManager.Instance.DialogPanel.SetDialog(21);
            UIManager.Instance.DialogPanel.StartDialog();
        }
        else if (stage == 13)
        {
            UIManager.Instance.DialogPanel.SetDialog(31);
            UIManager.Instance.DialogPanel.StartDialog();
        }
        else if (stage == 17)
        {
            UIManager.Instance.DialogPanel.SetDialog(41);
            UIManager.Instance.DialogPanel.StartDialog();
        }
    }
    
    public void LoadDialog() => LoadDialog(StageNumber);

    #endregion
}