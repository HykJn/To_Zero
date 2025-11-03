using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    #region ==== Properties ====

    public int GrideWidth { get; private set; } = 5;
    public int GrideHeight { get; private set; } = 4;
    public EditorMode CurrentMode { get; private set; } = EditorMode.PlaceTile;
    public string SelectedTileType { get; private set; } = "+1";

    #endregion

    #region ==== Fields ====

    [Header("References")]
    [SerializeField] private GridEditor gridEditor;

    [Header("Settings")]
    [SerializeField] private int defaultWidth = 5;
    [SerializeField] private int defaultHeight = 4;
    [SerializeField] private int moveCount = 10;
    [SerializeField] private int startValue = 0;

    private Dictionary<Vector2, string> tileData;
    private Vector2? startPosition;

    #endregion

    #region ==== Unity Events ====

    private void Start()
    {
        InitEditor();
    }

    #endregion

    #region ==== Methods ====

    private void InitEditor()
    {
        GrideWidth = defaultWidth;
        GrideHeight = defaultHeight;
        tileData = new Dictionary<Vector2, string>();

        gridEditor.InitGrid(GrideWidth, GrideHeight);
        Debug.Log("init editor");
    }

    public void OnCellClicked(Vector2 position)
    {
        switch (CurrentMode)
        {
            case EditorMode.PlaceTile:
                PlaceTile(position, SelectedTileType); break;

            case EditorMode.DeleteTile:
                DeleteTile(position); break;

            case EditorMode.SetStart:
                SetStartPosition(position); break;
        }
    }

    private void PlaceTile(Vector2 position, string tileType)
    {
        if(startPosition.HasValue && startPosition.Value == position)
        {
            return;
        }

        tileData[position] = tileType;
        gridEditor.UpdateCell(position, tileType);
    }

    private void DeleteTile(Vector2 position)
    {
        if (tileData.ContainsKey(position))
        {
            tileData.Remove(position);
            gridEditor.UpdateCell(position, "0");
        }
    }

    private void SetStartPosition(Vector2 position)
    {
        if (startPosition.HasValue)
        {
            Vector2 oldStart = startPosition.Value;
            gridEditor.UpdateCell(oldStart, tileData.ContainsKey(oldStart) ? tileData[oldStart] : "0");
        }
        
        startPosition = position;
        gridEditor.UpdateCell(position, "S");
    }

    public void SetMode(EditorMode mode)
    {
        CurrentMode = mode;
    }

    public void SelectTileType(string tileType)
    {
        SelectedTileType = tileType;
    }

    public void SetGrideSize(int width, int height)
    {
        GrideWidth = width;
        GrideHeight = height;

        tileData.Clear();
        startPosition = null;

        gridEditor.InitGrid(width, height);
    }

    public string GetTileAt(Vector2 position)
    {
        if (startPosition.HasValue && startPosition.Value == position) return "S";

        return tileData.ContainsKey(position) ? tileData[position] : "0";
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        float worldX = -(GrideWidth / 2) + (GrideWidth % 2 == 0 ? 0.5f : 0) + x;
        float worldY = (GrideHeight / 2) - (GrideHeight % 2 == 0 ? 0.5f : 0) + y;
        return new Vector2(worldX, worldY);
    }

    #endregion

    public enum EditorMode
    {
        PlaceTile,
        DeleteTile,
        SetStart,
    }
}
