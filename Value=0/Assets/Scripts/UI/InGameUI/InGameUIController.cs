using TMPro;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    #region ==========Properties==========

    public DialogPanel DialogPanel => dialogPanel;
    public PausePanel PausePanel => pausePanel;
    public DescriptionPanel DescriptionPanel => descriptionPanel;

    public int Stage
    {
        set => text_Stage.text = $"Stage {value}";
    }

    public int Moves
    {
        set => text_Moves.text = value.ToString();
    }

    public int Value
    {
        set
        {
            text_Value.text = value.ToString();
            text_Value.color = value switch
            {
                > 0 => new Color(100 / 255f, 200 / 255f, 255 / 255f),
                0 => Color.white,
                < 0 => new Color(255 / 255f, 100 / 255f, 100 / 255f),
            };
        }
    }

    #endregion

    [Header("Panels")]
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private DescriptionPanel descriptionPanel;

    [SerializeField] private DialogPanel dialogPanel;

    [SerializeField] private TMP_Text text_Stage;
    [SerializeField] private TMP_Text text_Moves;
    [SerializeField] private TMP_Text text_Value;

    public void Init()
    {
        text_Stage.text = string.Empty;
        text_Moves.text = string.Empty;
        text_Value.text = string.Empty;
    }
}