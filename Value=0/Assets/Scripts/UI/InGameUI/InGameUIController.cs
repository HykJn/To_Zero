using TMPro;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public DialogueSystem DialogPanel => dialogPanel;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject descriptionPanel;

    [SerializeField] private DialogueSystem dialogPanel;

    [SerializeField] private TMP_Text stageNum_T;
    [SerializeField] private TMP_Text value_T;
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
    }

    public void SetActive_PausePanel(bool isActive)
    {
        SoundManager.Instance.Play_UI_SFX(isActive ? UISFXID.PanelOpen : UISFXID.PanelClose);

        pausePanel.SetActive(isActive);
        if (isActive) UIManager.Instance.OpenPanel.Push(pausePanel);
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
        string sign = string.Empty;
        if (player.Value < 0) value_T.color = new Color(1, 75 / 255f, 75 / 255f);
        else if (player.Value == 0) value_T.color = Color.white;
        else
        {
            value_T.color = new Color(100 / 255f, 200 / 255f, 1);
            sign = "+";
        }
        value_T.text = sign + player.Value.ToString();
    }

    public void Update_MoveCount()
    {
        moveCount_T.text = player.Moves.ToString();
    }
}
