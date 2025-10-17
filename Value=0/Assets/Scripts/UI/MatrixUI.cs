using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatrixUI : MonoBehaviour
{
    #region =====Properties=====

    public int Stage
    {
        set
        {
            if (GameManager.Instance.CurrentBossStage)
            {
                text_Stage.gameObject.SetActive(false);
            }
            else
            {
                text_Stage.gameObject.SetActive(true);
                text_Stage.text = "Stage " + value;
            }
        }
    }

    public int Moves
    {
        set => text_Moves.text = value.ToString();
    }

    public int Value
    {
        set => text_Value.text = value.ToString();
    }

    // public BossUI BossUI => bossUI;
    public TutorialPanel TutorialPanel => tutorialPanel;
    public EndingPanel EndingPanel => endingPanel;

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private TutorialPanel tutorialPanel;
    [SerializeField] private EndingPanel endingPanel;
    // [SerializeField] private BossUI bossUI;

    [SerializeField] private TMP_Text text_Stage;
    [SerializeField] private TMP_Text text_Moves;
    [SerializeField] private TMP_Text text_Value;

    #endregion

    #region =====Unity Events=====

    private void OnEnable()
    {
        if (!UIManager.Instance.MatrixUI) UIManager.Instance.MatrixUI = this;
    }

    private void OnDisable()
    {
        UIManager.Instance.MatrixUI = null;
    }

    #endregion

    #region =====Methods=====

    #endregion
}