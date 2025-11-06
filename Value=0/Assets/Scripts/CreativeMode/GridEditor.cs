using UnityEngine;
using System.Collections.Generic;

public class GridEditor : MonoBehaviour
{
    #region ===== Fields =====

    [Header("Prefab")]
    [SerializeField] private GameObject editorCellPrefab;

    [Header("References")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private LevelEditor levelEditor;

    private Dictionary<Vector2, EditorCell> cells;

    #endregion

    #region ===== Methods =====

    public void InitializeGrid(int width, int height)
    {
        // 기존 그리드 제거
        ClearGrid();

        cells = new Dictionary<Vector2, EditorCell>();

        // Stage.cs와 동일한 위치 계산 방식
        float x = -(width / 2) + (width % 2 == 0 ? 0.5f : 0);
        float y = (height / 2) - (height % 2 == 0 ? 0.5f : 0);

        // 셀 생성
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2 pos = new Vector2(x + j, y - i);
                CreateCell(pos);
            }
        }

        Debug.Log($"Grid initialized: {width}x{height}, Total cells: {cells.Count}");
    }

    private void CreateCell(Vector2 position)
    {
        GameObject cellObj = Instantiate(editorCellPrefab, position, Quaternion.identity, gridParent);
        EditorCell cell = cellObj.GetComponent<EditorCell>();

        if (cell == null)
        {
            Debug.LogError("EditorCellPrefab에 EditorCell 컴포넌트가 없습니다!");
            return;
        }

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

        // GridParent의 모든 자식 제거 (안전장치)
        if (gridParent != null)
        {
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void UpdateCell(Vector2 position, string tileData)
    {
        Debug.Log($"GridEditor.UpdateCell: position={position}, tileData='{tileData}'");

        if (cells.TryGetValue(position, out EditorCell cell))
        {
            Debug.Log($"  → Cell found, calling cell.SetTileData...");
            cell.SetTileData(tileData);
        }
        else
        {
            Debug.LogWarning($"Cell at position {position} not found! Available cells: {cells.Count}");
        }
    }

    // EditorCell에서 호출
    public void OnCellClicked(EditorCell cell)
    {
        levelEditor.OnCellClicked(cell.Position);
    }

    #endregion
}