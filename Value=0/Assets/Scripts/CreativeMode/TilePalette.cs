using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TilePalette : MonoBehaviour
{
    #region ===== Fields =====

    [Header("References")]
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private Transform paletteContent;
    [SerializeField] private GameObject tileButtonPrefab;

    [Header("Tile Categories")]
    [SerializeField] private TileCategory[] categories;

    private Dictionary<string, Button> tileButtons;
    private Button currentSelectedButton;

    #endregion

    #region ===== Unity Events =====

    private void Start()
    {
        GeneratePalette();
        SelectTile("+");
    }

    #endregion

    #region ===== Methods =====

    private void GeneratePalette()
    {
        tileButtons = new Dictionary<string, Button>();

        foreach (TileCategory category in categories)
        {
            CreateCategoryHeader(category.categoryName);

            foreach (TileData tile in category.tiles)
            {
                CreateTileButton(tile);
            }
        }

    }

    private void CreateCategoryHeader(string categoryName)
    {
  
        GameObject headerObj = new GameObject(categoryName + "_Header");
        headerObj.transform.SetParent(paletteContent, false);

        LayoutElement layout = headerObj.AddComponent<LayoutElement>();
        layout.minHeight = 30;
        layout.preferredHeight = 30;

        TMP_Text text = headerObj.AddComponent<TextMeshProUGUI>();
        text.text = categoryName;
        text.fontSize = 20;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Left;
        text.color = Color.white;
    }

    private void CreateTileButton(TileData tileData)
    {

        GameObject buttonObj = Instantiate(tileButtonPrefab, paletteContent);
        Button button = buttonObj.GetComponent<Button>();

        TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            string displayText = FormatTileDisplay(tileData.tileCode);
            buttonText.text = displayText;
        }

        string tileCode = tileData.tileCode;
        button.onClick.AddListener(() => SelectTile(tileCode));

        tileButtons[tileCode] = button;
    }

    public void SelectTile(string tileCode)
    {
        Debug.Log($"TilePalette.SelectTile: '{tileCode}'");

        // 이전 선택 해제
        if (currentSelectedButton != null)
        {
            ColorBlock colors = currentSelectedButton.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
            colors.selectedColor = Color.white;
            currentSelectedButton.colors = colors;
        }

        // 새로운 선택
        if (tileButtons.TryGetValue(tileCode, out Button button))
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.yellow;
            colors.highlightedColor = new Color(1f, 1f, 0.7f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.5f);
            colors.selectedColor = Color.yellow;
            button.colors = colors;

            currentSelectedButton = button;
            levelEditor.SelectTileType(tileCode);

            Debug.Log($"Tile selected: {tileCode}");
        }
    }

    private string FormatTileDisplay(string tileCode)
    {
        string formatted = tileCode;
        formatted = formatted.Replace("*", "×");
        formatted = formatted.Replace("/", "÷");
        return formatted;
    }

    #endregion
}

[System.Serializable]
public class TileCategory
{
    public string categoryName;
    public TileData[] tiles;
}

[System.Serializable]
public class TileData
{
    public string tileCode;
    public string displayName;
}