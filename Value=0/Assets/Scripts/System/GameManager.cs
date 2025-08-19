using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    #region ==========Events==========

    public event Action OnRestart;

    #endregion

    #region ==========Properties==========

    public static GameManager Instance { get; private set; } = null;

    public int Stage
    {
        get => curStage;
        set
        {
            if (value < 1 || value > stages.Length + 1)
                throw new IndexOutOfRangeException();
            else if (value == stages.Length + 1)
                demoCanvas.SetActive(true);

            curStage = value;
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i].gameObject.SetActive(i == curStage - 1);
            }
        }
    }

    public Stage CurrentStage => stages[curStage - 1];

    #endregion

    #region ==========Fields==========

    [SerializeField] private Stage[] stages;
    [SerializeField] private int curStage;
    [SerializeField] private GameObject demoCanvas;

    [SerializeField] private DialogueSystem dialog;

    #endregion

    #region ==========Unity Methods==========

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        Stage = 1;
    }

    #endregion

    #region ==========Methods==========

    public void Transition(EventID id) => Camera.main.GetComponent<PPTransition>().Transition(id);

    public void Restart() => OnRestart?.Invoke();

    public void SetDialog()
    {
        DialogueData[] dialog = stages[Stage - 1].GetComponent<Stage>().Dialogs;
        if (dialog.Length > 0) UIManager.Instance.InGameUI.DialogPanel.SetDialog(dialog);
    }

    #endregion
}