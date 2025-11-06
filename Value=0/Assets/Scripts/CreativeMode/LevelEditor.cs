using UnityEngine;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{
    #region ===== Properties =====

    public int GridWidth { get; private set; } = 5;
    public int GridHeight { get; private set; } = 4;
    public EditorMode CurrentMode { get; private set; } = EditorMode.PlaceTile;
    public string SelectedTileType { get; private set; } = "+1";

    #endregion

    #region ===== Fields =====

    [Header("References")]
    [SerializeField] private GridEditor gridEditor;
    [SerializeField] private TileValueInputPanel tileValueInputPanel;

    [Header("Settings")]
    [SerializeField] private int defaultWidth = 5;
    [SerializeField] private int defaultHeight = 4;
    [SerializeField] private int moveCount = 10;
    [SerializeField] private int startValue = 0;

    private Dictionary<Vector2, string> tileData;
    private Vector2? startPosition;

    #endregion

    #region ===== Unity Events =====

    private void Start()
    {
        InitializeEditor();
    }

    #endregion

    #region ===== Methods =====

    private void InitializeEditor()
    {
        GridWidth = defaultWidth;
        GridHeight = defaultHeight;
        tileData = new Dictionary<Vector2, string>();

        // �׸��� ����
        gridEditor.InitializeGrid(GridWidth, GridHeight);

        Debug.Log($"Editor initialized: {GridWidth}x{GridHeight}");
    }

    // �׸��� ���� Ŭ���Ǿ��� �� ȣ��
    public void OnCellClicked(Vector2 position)
    {
        Debug.Log($"OnCellClicked: position={position}, mode={CurrentMode}, selectedTile='{SelectedTileType}'");

        switch (CurrentMode)
        {
            case EditorMode.PlaceTile:
                // ���� Ÿ������ Ȯ��
                bool isOperator = IsOperatorTile(SelectedTileType);
                Debug.Log($"  �� IsOperatorTile('{SelectedTileType}') = {isOperator}");

                if (isOperator)
                {
                    // TileValueInputPanel Ȯ��
                    if (tileValueInputPanel == null)
                    {
                        Debug.LogError("  �� TileValueInputPanel is NULL! Please connect it in Inspector.");
                        return;
                    }

                    Debug.Log($"  �� Showing input panel...");
                    // �Է�â ǥ��
                    tileValueInputPanel.Show(position, SelectedTileType, PlaceTile);
                }
                else
                {
                    Debug.Log($"  �� Placing special tile directly: '{SelectedTileType}'");
                    // Ư�� Ÿ���� �ٷ� ��ġ
                    PlaceTile(position, SelectedTileType);
                }
                break;

            case EditorMode.DeleteTile:
                DeleteTile(position);
                break;

            case EditorMode.SetStart:
                SetStartPosition(position);
                break;
        }
    }

    private bool IsOperatorTile(string tileType)
    {
        bool result = tileType == "+" || tileType == "-" || tileType == "*" || tileType == "/";
        Debug.Log($"IsOperatorTile check: '{tileType}' �� {result}");
        return result;
    }

    private void PlaceTile(Vector2 position, string tileType)
    {
        Debug.Log($"PlaceTile called: position={position}, tileType='{tileType}'");

        // ���� ��ġ���� Ÿ�� ��ġ �Ұ�
        if (startPosition.HasValue && startPosition.Value == position)
        {
            Debug.LogWarning("���� ��ġ���� Ÿ���� ��ġ�� �� �����ϴ�.");
            return;
        }

        tileData[position] = tileType;
        Debug.Log($"  �� tileData updated, calling gridEditor.UpdateCell...");
        gridEditor.UpdateCell(position, tileType);

        Debug.Log($"Tile placed at {position}: {tileType}");
    }

    private void DeleteTile(Vector2 position)
    {
        if (tileData.ContainsKey(position))
        {
            tileData.Remove(position);
            gridEditor.UpdateCell(position, "0");
            Debug.Log($"Tile deleted at {position}");
        }
    }

    private void SetStartPosition(Vector2 position)
    {
        // ���� ���� ��ġ ����
        if (startPosition.HasValue)
        {
            Vector2 oldStart = startPosition.Value;
            gridEditor.UpdateCell(oldStart, tileData.ContainsKey(oldStart) ? tileData[oldStart] : "0");
        }

        // �� ���� ��ġ ����
        startPosition = position;
        gridEditor.UpdateCell(position, "S");

        Debug.Log($"Start position set at {position}");
    }

    // UI���� ȣ���� �޼����
    public void SetMode(EditorMode mode)
    {
        CurrentMode = mode;
        Debug.Log($"Editor mode changed to: {mode}");
    }

    public void SelectTileType(string tileType)
    {
        SelectedTileType = tileType;
        Debug.Log($"SelectTileType: '{tileType}' (length: {tileType.Length})");

        // ���ڸ� �ϳ��� ��� (�����̳� ���� ���� Ȯ�ο�)
        for (int i = 0; i < tileType.Length; i++)
        {
            Debug.Log($"  [{i}] = '{tileType[i]}' (code: {(int)tileType[i]})");
        }
    }

    public void SetGridSize(int width, int height)
    {
        GridWidth = width;
        GridHeight = height;

        // ���� ������ �ʱ�ȭ
        tileData.Clear();
        startPosition = null;

        // �׸��� �����
        gridEditor.InitializeGrid(width, height);

        Debug.Log($"Grid resized to {width}x{height}");
    }

    // Ÿ�� ������ �������� (�����)
    public string GetTileAt(Vector2 position)
    {
        if (startPosition.HasValue && startPosition.Value == position)
            return "S";

        return tileData.ContainsKey(position) ? tileData[position] : "0";
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        float worldX = -(GridWidth / 2) + (GridWidth % 2 == 0 ? 0.5f : 0) + x;
        float worldY = (GridHeight / 2) - (GridHeight % 2 == 0 ? 0.5f : 0) - y;
        return new Vector2(worldX, worldY);
    }

    #endregion
}

public enum EditorMode
{
    PlaceTile,      // Ÿ�� ��ġ
    DeleteTile,     // Ÿ�� ����
    SetStart,       // ���� ��ġ ����
}