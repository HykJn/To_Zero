using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Stage : MonoBehaviour
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========
    [SerializeField, TextArea(6, 6)] private string stageMap;
    [SerializeField] private int startNumber, moves;
    [SerializeField] private Vector2 startPos;
    private List<GameObject> objs;
    private List<SwapTile> swapTiles;
    #endregion

    #region ==========Unity Methods==========
    private void OnEnable()
    {
        Init();
        Restart();
    }

    private void OnDisable()
    {
        UnloadStage();
    }
    #endregion

    #region ==========Methods==========
    private void Init()
    {
        objs = new();
        swapTiles = new();
        LoadStage();
    }

    private void LoadStage()
    {
        string[] lines = stageMap.Split("\n");
        int height = lines.Length, width = lines[0].Split(' ').Length;

        if (height % 2 != 0) this.transform.position += Vector3.down * 0.5f;
        if (width % 2 != 0) this.transform.position += Vector3.left * 0.5f;

        int left = -width / 2, top = height % 2 == 0 ? (height / 2) - 1 : height / 2;

        for (int y = 0; y < height; y++)
        {
            string[] tiles = lines[y].Split(' ');
            for (int x = 0; x < width; x++)
            {
                if (tiles[x] == "Start")
                {
                    startPos = new Vector2(left + x, top - y);
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, startPos).GetComponent<OperationTile>();
                    tile.Operator = Operator.None;
                    tile.Value = 0;
                    objs.Add(tile.gameObject);
                }
                else if (tiles[x][0] == 'S' || tiles[x][0] == 's')
                {
                    SwapTile swapTile = ObjectManager.Instance.GetObject(ObjectID.SwapTile, new Vector2(left + x, top - y)).GetComponent<SwapTile>();
                    swapTile.SetTile(tiles[x][1..]);
                    swapTiles.Add(swapTile);
                    objs.Add(swapTile.gameObject);
                }
                else if (tiles[x] == "W" || tiles[x] == "w")
                {
                    objs.Add(ObjectManager.Instance.GetObject(ObjectID.Wall, new Vector2(left + x, top - y)));
                }
                else
                {
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, new Vector2(left + x, top - y)).GetComponent<OperationTile>();

                    if (tiles[x] == "P" || tiles[x] == "p")
                    {
                        tile.Operator = Operator.Portal;
                        tile.Value = 0;
                    }
                    else
                    {
                        tile.Operator = tiles[x][0] switch
                        {
                            '+' => Operator.Add,
                            '-' => Operator.Sub,
                            '*' => Operator.Mul,
                            '/' => Operator.Div,
                            '=' => Operator.Equal,
                            '!' => Operator.Not,
                            '>' => Operator.Greater,
                            '<' => Operator.Less,
                            _ => throw new InvalidOperationException("Invalid operator in stage map.")
                        };
                        tile.Value = int.Parse(tiles[x][1..]);
                    }

                    objs.Add(tile.gameObject);
                }
            }
        }
    }

    private void UnloadStage()
    {
        foreach (GameObject obj in objs)
        {
            obj.SetActive(false);
        }
        objs.Clear();
    }

    public void Restart()
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.transform.position = startPos;
        player.StartNumber = startNumber;
        player.Moves = moves;
    }

    public void SwapTiles()
    {
        if (swapTiles.Count == 0) return;
        foreach (SwapTile tile in swapTiles)
        {
            tile.Swap();
        }
    }
    #endregion
}
