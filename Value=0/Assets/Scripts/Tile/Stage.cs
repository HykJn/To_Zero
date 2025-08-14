using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using TMPro;

public class Stage : MonoBehaviour
{
    #region ==========Properties==========
    public DialogueData[] Dialogs => dialogs;
    #endregion

    #region ==========Fields==========
    [SerializeField, TextArea(6, 6)] private string stageMap;
    [SerializeField] private int startNumber, moves;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private DroneInfo[] droneInfo;
    private List<Drone> drones;
    private List<GameObject> objs;
    [SerializeField] private List<SwapTile> swapTiles;
    [SerializeField] private DialogueData[] dialogs;
    #endregion

    #region ==========Unity Methods==========
    private void OnEnable()
    {
        Init();
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.transform.position = startPos;
        player.Value = startNumber;
        player.Moves = moves;
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
        drones = new();
        LoadStage();
    }

    private void LoadStage()
    {
        //Load Tiles
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
                    tile.OnPlayer = true;
                    objs.Add(tile.gameObject);
                }
                else if (tiles[x] == "W" || tiles[x] == "w")
                {
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, pos).GetComponent<OperationTile>();
                    tile.Operator = Operator.None;
                    tile.Value = 0;
                    tile.GetComponent<Animator>().enabled = false;
                    objs.Add(tile.gameObject);
                    objs.Add(ObjectManager.Instance.GetObject(ObjectID.Wall, (Vector3)pos + Vector3.back));
                }
                else if (tiles[x][0] == 'S' || tiles[x][0] == 's')
                {
                    SwapTile swapTile = ObjectManager.Instance.GetObject(ObjectID.SwapTile, pos).GetComponent<SwapTile>();
                    swapTile.SetTile(tiles[x][1..]);
                    swapTiles.Add(swapTile);
                }
                else
                {
                    OperationTile tile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, pos).GetComponent<OperationTile>();

                    if (tiles[x] == "P" || tiles[x] == "p")
                    {
                        //TODO: Fix later
                        Camera.main.GetComponent<PPTransition>().Portal = (Vector3)pos + Vector3.back;
                        //GameObject portal = ObjectManager.Instance.GetObject(ObjectID.Portal, pos);
                        //objs.Add(portal);
                        tile.Operator = Operator.Portal;
                        tile.Value = 0;
                    }
                    else
                    {
                        GameObject obj = null;
                        if (tiles[x][0] == 'B' || tiles[x][0] == 'b')
                        {
                            tile.GetComponent<Animator>().enabled = false;
                            obj = ObjectManager.Instance.GetObject(ObjectID.Box, (Vector3)pos + Vector3.back);
                            objs.Add(obj);
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

                        if (obj != null)
                        {
                            obj.GetComponentInChildren<TMP_Text>().text =
                                tile.GetComponentInChildren<TMP_Text>().text;
                        }
                    }

                    objs.Add(tile.gameObject);
                }
            }
        }

        //Load Drone
        if (droneInfo.Length > 0)
        {
            foreach (DroneInfo info in droneInfo)
            {
                Drone drone = ObjectManager.Instance.GetObject(ObjectID.Drone, info.start).GetComponent<Drone>();
                drone.Init(info.start, info.direction, info.steps);
                drones.Add(drone);
            }
        }
    }

    private void UnloadStage()
    {
        foreach (GameObject obj in objs)
        {
            if (obj != null) obj.SetActive(false);
        }
        objs.Clear();

        foreach (SwapTile tile in swapTiles)
        {
            if (tile != null) tile.gameObject.SetActive(false);
        }
        swapTiles.Clear();

        foreach (Drone drone in drones)
        {
            if (drone != null) drone.gameObject.SetActive(false);
        }
        drones.Clear();
    }

    public void Restart()
    {
        UnloadStage();
        LoadStage();
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.transform.position = startPos;
        player.Value = startNumber;
        player.Moves = moves;

        foreach (Drone drone in drones)
        {
            drone.Init();
        }
    }

    public void SwapTiles()
    {
        if (swapTiles.Count == 0) return;
        foreach (SwapTile tile in swapTiles)
        {
            tile.Swap();
        }
    }

    public void MoveDrone()
    {
        if (drones.Count == 0) return;
        foreach (Drone drone in drones)
            drone.Move();
    }
    #endregion

    [Serializable]
    private struct DroneInfo
    {
        public Vector2 start;
        public Drone.Direction direction;
        public int steps;
    }
}
