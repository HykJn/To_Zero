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
    [SerializeField] private DroneInfo[] droneInfo;
    [SerializeField] private DialogueData[] dialogs;

    private Dictionary<Vector3, OperationTile> _tileMap;
    private Vector3 _startPos;
    private List<GameObject> _objs;

    #endregion

    #region ==========Unity Methods==========

    private void OnEnable()
    {
        GameManager.Instance.OnRestart += Restart;
        LoadStage();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRestart -= Restart;
        UnloadStage();
    }

    #endregion

    #region ==========Methods==========

    private void LoadStage()
    {
        //Initialize
        _tileMap = new();
        _objs = new();

        string[] rows = stageMap.Split('\n');
        int height = rows.Length, width = rows[0].Length;
        float top = height / 2f - 0.5f, left = -(width / 2f - 0.5f);

        //Load Tiles
        for (int y = 0; y < rows.Length; y++)
        {
            string[] columns = rows[y].Split(' ');
            for (int x = 0; x < columns.Length; x++)
            {
                if (columns[x] == "0") continue;

                Vector3 position = new(left + x, top - y);
                if (columns[x][0] == 'b')
                {
                    Box box = ObjectManager.Instance.GetObject(ObjectID.Box, position).GetComponent<Box>();
                    box.Init(position);
                    _objs.Add(box.gameObject);

                    columns[x] = columns[x][1..];
                }

                if (columns[x][0] == 's')
                {
                    SwapTile sTile = ObjectManager.Instance.GetObject(ObjectID.SwapTile, position)
                        .GetComponent<SwapTile>();

                    _tileMap.Add(position, sTile);
                    sTile.SetTile(columns[x][1..]);
                }
                else
                {
                    OperationTile newTile = ObjectManager.Instance.GetObject(ObjectID.OperationTile, position)
                        .GetComponent<OperationTile>();
                    _tileMap.Add(position, newTile);

                    switch (columns[x])
                    {
                        case "w":
                            newTile.TileType = TileType.None;
                            newTile.Value = 0;
                            _objs.Add(ObjectManager.Instance.GetObject(ObjectID.Wall, position));
                            break;
                        case "S":
                            _startPos = position;
                            newTile.TileType = TileType.Start;
                            newTile.Value = 0;
                            break;
                        case "P":
                            newTile.TileType = TileType.Portal;
                            newTile.Value = 0;
                            break;
                        default:
                            newTile.TileType = columns[x][0] switch
                            {
                                '+' => TileType.Add, '-' => TileType.Sub, '*' => TileType.Mul, '/' => TileType.Div,
                                '=' => TileType.Equal, '!' => TileType.Not, '>' => TileType.Greater,
                                '<' => TileType.Less, _ => TileType.None,
                            };
                            newTile.Value = int.Parse(columns[x][1..]);
                            break;
                    }
                }
            }
        }

        //Load Drones
        foreach (DroneInfo info in droneInfo)
        {
            Drone drone = ObjectManager.Instance.GetObject(ObjectID.Drone).GetComponent<Drone>();
            drone.Init(info.start, info.direction, info.steps);
            _objs.Add(drone.gameObject);
        }
    }

    private void UnloadStage()
    {
        foreach (OperationTile tile in _tileMap.Values)
            tile.gameObject.SetActive(false);
        _tileMap.Clear();

        foreach (GameObject obj in _objs)
            obj.SetActive(false);
        _objs.Clear();
    }

    public bool IsMovable(Vector3 position)
    {
        return GetTile(position)?.TileType != TileType.None;
    }

    public OperationTile GetTile(Vector3 position)
    {
        return _tileMap.GetValueOrDefault(position, null);
    }

    private void Restart() => GameObject.FindWithTag("Player").transform.position = _startPos;

    #endregion

    [Serializable]
    private struct DroneInfo
    {
        public Vector2 start;
        public Drone.Direction direction;
        public int steps;
    }
}