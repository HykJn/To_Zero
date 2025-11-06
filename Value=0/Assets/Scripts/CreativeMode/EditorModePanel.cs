using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LevelEditor;

public class EditorModePanel : MonoBehaviour
{
    #region ===== Fields =====

    [Header("References")]
    [SerializeField] private LevelEditor levelEditor;

    [Header("Mode Buttons")]
    [SerializeField] private Button placeTileButton;
    [SerializeField] private Button deleteTileButton;
    [SerializeField] private Button setStartButton;

    [Header("Info Text")]
    [SerializeField] private TMP_Text modeInfoText;

    private Button currentSelectedButton;

    #endregion

    #region ===== Unity Events =====

    private void Start()
    {
       
        placeTileButton.onClick.AddListener(() => SetMode(EditorMode.PlaceTile));
        deleteTileButton.onClick.AddListener(() => SetMode(EditorMode.DeleteTile));
        setStartButton.onClick.AddListener(() => SetMode(EditorMode.SetStart));

        SetMode(EditorMode.PlaceTile);
    }

    #endregion

    #region ===== Methods =====

    public void SetMode(EditorMode mode)
    {
     
        levelEditor.SetMode(mode);

      
        UpdateButtonHighlight(mode);
        UpdateInfoText(mode);
    }

    private void UpdateButtonHighlight(EditorMode mode)
    {
      
        ResetButtonColor(placeTileButton);
        ResetButtonColor(deleteTileButton);
        ResetButtonColor(setStartButton);

      
        Button selectedButton = mode switch
        {
            EditorMode.PlaceTile => placeTileButton,
            EditorMode.DeleteTile => deleteTileButton,
            EditorMode.SetStart => setStartButton,
            _ => placeTileButton
        };

        HighlightButton(selectedButton);
        currentSelectedButton = selectedButton;
    }

    private void ResetButtonColor(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        button.colors = colors;
    }

    private void HighlightButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.5f, 1f, 0.5f); 
        button.colors = colors;
    }

    private void UpdateInfoText(EditorMode mode)
    {
        if (modeInfoText == null) return;

        modeInfoText.text = mode switch
        {
            EditorMode.PlaceTile => "모드: 타일 배치 - 타일을 선택하고 셀을 클릭하세요",
            EditorMode.DeleteTile => "모드: 타일 삭제 - 삭제할 셀을 클릭하세요",
            EditorMode.SetStart => "모드: 시작 위치 설정 - 시작 위치를 클릭하세요",
            _ => "모드: 알 수 없음"
        };
    }

    #endregion
}