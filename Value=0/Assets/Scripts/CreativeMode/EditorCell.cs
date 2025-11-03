using UnityEngine;
using TMPro;
using NUnit.Framework.Constraints;


public class EditorCell : MonoBehaviour
{
    #region ==== Properties ====

    public Vector2 Position {  get; private set; } 
    public string TileData {  get; private set; }

    #endregion

    #region ==== Fields ====

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Colors")]
    [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color tileColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField] private Color startColor = new Color(0.2f, 1f, 0.2f, 0.8f);
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.5f, 0.8f);

    private GridEditor gridEditor;
    private Color currentColor;

    #endregion

    #region ==== Unity Events ====

    private void OnMouseDown()
    {
        if (gridEditor != null)
            gridEditor.OnCellClicked(this);
    }

    private void OnMouseEnter()
    {
        if (backgroundRenderer != null)
            backgroundRenderer.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (backgroundRenderer != null)
            backgroundRenderer.color = currentColor;
    }

    #endregion

    #region ==== Methods ====

    public void Init(Vector2 position, GridEditor editor)
    {
        Position = position;
        gridEditor = editor;
        SetTileData("0");

        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (displayText == null) displayText = GetComponentInChildren<TMP_Text>();

        if(backgroundRenderer == null)
        {
            Transform bgTransform = transform.Find("Background");
            if(bgTransform != null)
                backgroundRenderer = bgTransform.GetComponent<SpriteRenderer>();
        }
    }

    public void SetTileData(string data)
    {
        TileData = data;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (displayText == null) return;

        if (string.IsNullOrEmpty(TileData) || TileData == "0")
        {
            displayText.text = "";
            currentColor = emptyColor;
        }
        else if (TileData == "S")
        {
            displayText.text = "START";
            displayText.fontSize = 3;
            currentColor = startColor;
        }
        else if (TileData == "P")
        {
            displayText.text = "PORTAL";
            displayText.fontSize = 3;
            currentColor = new Color(0.5f, 0.2f, 1f, 0.8f);
        }
        else if (TileData == "C")
        {
            displayText.text = "CUBE";
            displayText.fontSize = 3;
            currentColor = new Color(0.8f, 0.5f, 0.2f, 0.8f);
        }
        else if (TileData == "N")
        {
            displayText.text = "NONE";
            displayText.fontSize = 3;
            currentColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
        else if (TileData.StartsWith("F"))
        {
            displayText.text = "F\n" + TileData.Substring(1);
            displayText.fontSize = 3;
            currentColor = new Color(1f, 0.3f, 0.3f, 0.8f);
        }
        else if (TileData.Contains(","))
        {
            displayText.text = TileData;
            displayText.fontSize = 2.5f;
            currentColor = new Color(0.8f, 0.3f, 1f, 0.8f);
        }
        else
        {
            displayText.text = FormatOperator(TileData);
            displayText.fontSize = 4;
            currentColor = tileColor;
        }

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = currentColor;
        }
    }

    private string FormatOperator(string tileData)
    {
        if (string.IsNullOrEmpty(tileData)) return "";

        string formatted = tileData;
        formatted = formatted.Replace("*", "x");
        formatted = formatted.Replace("/", "¡À");
        formatted = formatted.Replace("=", "=");
        formatted = formatted.Replace("!", "¡Á");
        formatted = formatted.Replace(">", ">");
        formatted = formatted.Replace("<", "<");

        return formatted;
    }


    #endregion
}
