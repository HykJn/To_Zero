using TMPro;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private TMP_Text stageNum_T;
    [SerializeField] private TMP_Text currentMove_T;
    [SerializeField] private TMP_Text moveCount_T;

    private Player player;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        InitializeTexts();
    }

    void Update()
    {
        Update_CurrentMove();
        Update_MoveCount();
    }

    public void SetActive_PausePanel(bool isActive)
    {
        pausePanel.SetActive(isActive);
    }

    private void InitializeTexts()
    {
        Update_StageNum();
        Update_CurrentMove();
        Update_MoveCount();
    }

    public void Update_StageNum()
    {
        stageNum_T.text = "Stage " + GameManager.Instance.Stage.ToString();
    }

    public void Update_CurrentMove()
    {
        currentMove_T.text = player.StartNumber.ToString();
    }

    public void Update_MoveCount()
    {
        moveCount_T.text = player.Moves.ToString();
    }
}
