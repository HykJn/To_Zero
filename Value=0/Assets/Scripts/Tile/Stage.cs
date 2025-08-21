using UnityEngine;
using System;
using System.Collections.Generic;
using static GlobalDefines;

public class Stage : MonoBehaviour
{
    #region ==========Properties==========

    public DialogueData[] Dialogs => dialogs;
    public Vector3 StartPos { get; private set; }
    public int StartNumber => startNumber;
    public int Moves => moves;

    #endregion

    #region ==========Fields==========

    [Header("Stage Configuration"), SerializeField, TextArea(6, 6)]
    private string stageMap;

    [SerializeField] private int startNumber, moves;
    [SerializeField] private DroneInfo[] droneInfo;
    [SerializeField] private DialogueData[] dialogs;

    private Dictionary<Vector3, Tile> _tileMap;
    private List<Drone> _drones;
    private List<GameObject> _objs;

    #endregion

    #region ==========Unity Methods==========

    private void OnEnable()
    {
        Restart();
        GameManager.Instance.OnPlayerMove += OnPlayerMove;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerMove -= OnPlayerMove;
    }

    #endregion

    #region ==========Methods==========

    public void Init()
    {
        _tileMap = new();
        _drones = new();
        _objs = new();
        LoadStage();

        foreach (Tile tile in _tileMap.Values)
            tile.Init();
        foreach (Drone drone in _drones)
            drone.Restart();
    }

    public void Restart()
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.transform.position = StartPos;
        player.Moves = moves;
        player.Value = startNumber;

        foreach (Tile tile in _tileMap.Values)
            tile.Restart();
        foreach (Drone drone in _drones)
            drone.Restart();
    }

    public void OnPlayerMove()
    {
        foreach (Tile tile in _tileMap.Values)
            tile.Swap();
        foreach (Drone drone in _drones)
            drone.Move();
    }

    private void LoadStage()
    {
        string[] rows = stageMap.Split('\n');
        int height = rows.Length, width = rows[0].Split(' ').Length;
        float top = height / 2f - 0.5f, left = -(width / 2f - 0.5f);

        //Load Tiles
        for (int y = 0; y < rows.Length; y++)
        {
            string[] columns = rows[y].Split(' ');
            for (int x = 0; x < columns.Length; x++)
            {
                if (columns[x] == "0") continue;

                Vector3 position = new(left + x, top - y);
                if (columns[x] == "S") StartPos = position;

                Tile tile = ObjectManager.Instance.GetObject(ObjectID.Tile, position).GetComponent<Tile>();
                _tileMap.Add(position, tile);

                if (columns[x][0] == 'b')
                {
                    Box box = ObjectManager.Instance.GetObject(ObjectID.Box, position).GetComponent<Box>();
                    tile.Box = box;
                    _objs.Add(box.gameObject);
                    box.Init(position);
                    columns[x] = columns[x][1..];
                }

                tile.SetTile(columns[x]);
            }
        }

        //Load Drones
        foreach (DroneInfo info in droneInfo)
        {
            Drone drone = ObjectManager.Instance.GetObject(ObjectID.Drone).GetComponent<Drone>();
            drone.Init(info.start, info.direction, info.steps);
            _drones.Add(drone);
        }
    }

    private void UnloadStage()
    {
        foreach (Tile tile in _tileMap.Values)
            tile.gameObject.SetActive(false);
        _tileMap.Clear();
        
        foreach(Drone drone in _drones)
            drone.gameObject.SetActive(false);
        _drones.Clear();

        foreach (GameObject obj in _objs)
            obj.SetActive(false);
        _objs.Clear();
    }

    public Tile GetTile(Vector3 position)
    {
        return _tileMap.GetValueOrDefault(position, null);
    }

    public Box GetBox(Vector3 position)
    {
        Box box = GetTile(position).Box;
        return box ? box : null;
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