using UnityEngine;
using TMPro;

public class EditorCell : MonoBehaviour
{
    #region ===== Properties =====

    public Vector2 Position { get; private set; }
    public string TileData { get; private set; }

    #endregion

    #region ===== Fields =====

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

    #region ===== Unity Events =====

    private void OnMouseDown()
    {
        if (gridEditor != null)
        {
            gridEditor.OnCellClicked(this);
        }
    }

    private void OnMouseEnter()
    {
        // 호버 효과
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = hoverColor;
        }
    }

    private void OnMouseExit()
    {
        // 원래 색상으로 복구
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = currentColor;
        }
    }

    #endregion

    #region ===== Methods =====

    public void Init(Vector2 position, GridEditor editor)
    {
        Position = position;
        gridEditor = editor;

        // 기본 스프라이트 설정 (없으면 생성)
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // DisplayText 찾기 (3D TextMeshPro)
        if (displayText == null)
        {
            // GetComponentInChildren은 TMP_Text도 찾을 수 있음
            displayText = GetComponentInChildren<TMP_Text>();

            if (displayText == null)
            {

                // TextMeshPro 3D 자동 생성
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(transform, false);
                textObj.transform.localPosition = new Vector3(0, 0, -0.1f);

                // TextMeshPro 컴포넌트 추가 (3D 버전)
                displayText = textObj.AddComponent<TextMeshPro>();
                displayText.fontSize = 4;
                displayText.alignment = TextAlignmentOptions.Center;
                displayText.color = Color.black;

                // Renderer 설정
                var renderer = textObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = 2;
                }
            }
        }

        if (backgroundRenderer == null)
        {
            // Background용 자식 오브젝트 찾기
            Transform bgTransform = transform.Find("Background");
            if (bgTransform != null)
                backgroundRenderer = bgTransform.GetComponent<SpriteRenderer>();
        }

        SetTileData("0");
    }

    public void SetTileData(string data)
    {
        TileData = data;
        UpdateVisual();
    }

    private void UpdateVisual()
    {

        if (string.IsNullOrEmpty(TileData) || TileData == "0")
        {
            // 빈 셀
            Debug.Log($"  → Empty cell");
            displayText.text = "";
            currentColor = emptyColor;
        }
        else if (TileData == "S")
        {
            // 시작 위치
            Debug.Log($"  → Start position");
            displayText.text = "START";
            displayText.fontSize = 3;
            currentColor = startColor;
        }
        else if (TileData == "P")
        {
            // 포탈
            Debug.Log($"  → Portal");
            displayText.text = "PORTAL";
            displayText.fontSize = 3;
            currentColor = new Color(0.5f, 0.2f, 1f, 0.8f);
        }
        else if (TileData == "C")
        {
            // 큐브
            Debug.Log($"  → Cube");
            displayText.text = "CUBE";
            displayText.fontSize = 3;
            currentColor = new Color(0.8f, 0.5f, 0.2f, 0.8f);
        }
        else if (TileData == "N")
        {
            // None
            Debug.Log($"  → None");
            displayText.text = "NONE";
            displayText.fontSize = 3;
            currentColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
        else if (TileData.StartsWith("F"))
        {
            // Firewall
            Debug.Log($"  → Firewall");
            displayText.text = "F\n" + TileData.Substring(1);
            displayText.fontSize = 3;
            currentColor = new Color(1f, 0.3f, 0.3f, 0.8f);
        }
        else if (TileData.Contains(","))
        {
            // SwapTile
            Debug.Log($"  → SwapTile");
            displayText.text = TileData;
            displayText.fontSize = 2.5f;
            currentColor = new Color(0.8f, 0.3f, 1f, 0.8f);
        }
        else
        {
            // 일반 연산 타일
            string formatted = FormatOperator(TileData);
            Debug.Log($"  → Operation tile: '{TileData}' formatted to '{formatted}'");
            displayText.text = formatted;
            displayText.fontSize = 4;
            currentColor = tileColor;
        }

        // 색상 적용
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = currentColor;
        }

    }

    private string FormatOperator(string tileData)
    {
        if (string.IsNullOrEmpty(tileData)) return "";

        // 연산자 치환
        string formatted = tileData;
        formatted = formatted.Replace("*", "×");
        formatted = formatted.Replace("/", "÷");
        formatted = formatted.Replace("=", "=");
        formatted = formatted.Replace("!", "≠");
        formatted = formatted.Replace(">", ">");
        formatted = formatted.Replace("<", "<");

        return formatted;
    }

    #endregion
}