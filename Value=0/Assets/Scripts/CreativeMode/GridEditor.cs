using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GridEditor : MonoBehaviour
{
    #region ==== Fields ====

    [Header("Prefab")]
    [SerializeField] private GameObject editorCellPrefab;

    [Header("References")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private LevelEditor levelEditor;

    private Dictionary<Vector2, EditorCell> cells;

    #endregion

    #region ==== Methods ====

    public void InitGrid(int width, int height)
    {
        ClearGrid();

        cells = new Dictionary<Vector2, EditorCell>();

        float x = -(width / 2) + (width % 2 == 0 ? 0.5f : 0);
        float y = (height / 2) - (height % 2 == 0 ? 0.5f : 0);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2 pos = new Vector2(x + j, y - i);
                CreateCell(pos);
            }
        }
    }

    private void CreateCell(Vector2 position)
    {
        GameObject cellObj = Instantiate(editorCellPrefab, position, Quaternion.identity, gridParent);
        EditorCell cell = cellObj.GetComponent<EditorCell>();

        if (cell == null) return;
        cell.Init(position, this);
        cells[position] = cell;
    }

    private void ClearGrid()
    {
        if (cells != null)
        {
            foreach (var cell in cells.Values)
            {
                if (cell != null && cell.gameObject != null)
                {
                    Destroy(cell.gameObject);
                }
            }
            cells.Clear();
        }

        if (gridParent != null)
        {
            foreach(Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void UpdateCell(Vector2 position, string tileData)
    {
        if (cells.TryGetValue(position, out EditorCell cell))
        {
            cell.SetTileData(tileData);
        }
    }

    public void OnCellClicked(EditorCell cell)
    {
        levelEditor.OnCellClicked(cell.Position);
    }

    #endregion
}
