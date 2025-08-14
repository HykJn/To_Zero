using TMPro;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject descriptionPanel;

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
        Update_StageNum();
        Update_Value();
        Update_MoveCount();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetActive_PausePanel(!pausePanel.activeSelf);
        }
    }

    public void SetActive_PausePanel(bool isActive)
    {
        pausePanel.SetActive(isActive);
    }
    public void SetActive_DescriptionPanel(bool isActive)
    {
        descriptionPanel.SetActive(isActive);
    }

    private void InitializeTexts()
    {
        Update_StageNum();
        Update_Value();
        Update_MoveCount();
    }

    public void Update_StageNum()
    {
        stageNum_T.text = "Stage " + GameManager.Instance.Stage.ToString();
    }

    public void Update_Value()
    {
        string sign;
        if (player.Value < 0)
        {
            currentMove_T.color = Color.red;
            sign = "-";
        }
        else if (player.Value == 0)
        {
            currentMove_T.color = Color.white;
            sign = "";
        }
        else
        {
            currentMove_T.color = Color.blue;
            sign = "+";
        }
        currentMove_T.text = sign + player.Value.ToString();
    }

    public void Update_MoveCount()
    {
        moveCount_T.text = player.Moves.ToString();
    }
}
