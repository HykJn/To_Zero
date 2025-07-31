using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using TMPro;

public class Stage : MonoBehaviour
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========
    [SerializeField, TextArea(6, 6)] private string stageMap;
    [SerializeField] private int startNumber, moves;
    [SerializeField] private Vector3 startPos;
    private List<GameObject> objs;
    private List<SwapTile> swapTiles;
    private List<Box> boxes;
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
        boxes = new();
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
                if (tiles[x] == "0" || tiles[x] == "N" || tiles[x] == "0") continue;
                Vector2 pos = new(left + x, top - y);
                if (tiles[x] == "Start")
                {
                    startPos = new Vector3(left + x, top - y);
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, pos).GetComponent<OperationTile>();
                    tile.Operator = Operator.None;
                    tile.Value = 0;
                    objs.Add(tile.gameObject);
                }
                else if (tiles[x] == "W" || tiles[x] == "w")
                {
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, pos).GetComponent<OperationTile>();
                    tile.Operator = Operator.None;
                    tile.Value = 0;
                    objs.Add(tile.gameObject);
                    objs.Add(ObjectManager.Instance.GetObject(ObjectID.Wall, pos));
                }
                else if (tiles[x][0] == 'S' || tiles[x][0] == 's')
                {
                    SwapTile swapTile = ObjectManager.Instance.GetObject(ObjectID.SwapTile, pos).GetComponent<SwapTile>();
                    swapTile.SetTile(tiles[x][1..]);
                    swapTiles.Add(swapTile);
                    objs.Add(swapTile.gameObject);
                }
                else
                {
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, pos).GetComponent<OperationTile>();

                    if (tiles[x] == "P" || tiles[x] == "p")
                    {
                        tile.Operator = Operator.Portal;
                        tile.Value = 0;
                    }
                    else
                    {
                        if (tiles[x][0] == 'B' || tiles[x][0] == 'b')
                        {
                            GameObject obj = ObjectManager.Instance.GetObject(ObjectID.Box, pos);
                            objs.Add(obj);
                            boxes.Add(obj.GetComponent<Box>());
                            tiles[x] = tiles[x][1..];
                        }

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
        foreach (Box box in boxes)
        {
            box.UpdateValue();
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
