using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GLOBAL;

public class Stage : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("Configuration")]
    [SerializeField] private int moveCount;
    [SerializeField] private int startValue;
    [SerializeField, TextArea(5, 20)] private string tileInfo;
    [SerializeField] private EnemyInfo[] enemyInfo;

    private Dictionary<Vector2, Tile> _tileMap;
    private Dictionary<Vector2, Firewall> _firewalls;
    private List<Enemy> _enemies;
    private Vector2 _startPos;

    #endregion

    #region =====Unity Events=====

    private void OnEnable()
    {
        LoadStage();
        Init();
        GameManager.Instance.OnRestart += OnRestart;
    }

    private void OnDisable()
    {
        UnloadStage();
        GameManager.Instance.OnRestart -= OnRestart;
    }

    #endregion

    #region =====Methods=====

    private void LoadStage()
    {
        //Init Tilemap's info
        string[] lines = tileInfo.Split('\n');
        int width = lines[0].Split(' ').Length;
        int height = lines.Length;

        float x = -(width / 2) + (width % 2 == 0 ? 0.5f : 0);
        float y = (height / 2) - (height % 2 == 0 ? 0.5f : 0);

        _tileMap = new();
        _firewalls = new();
        _enemies = new();

        //Load Tiles
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');
            for (int j = 0; j < parts.Length; j++)
            {
                string part = parts[j];
                Vector2 pos = new(x + j, y - i);

                if (part.Equals("0")) continue;
                if (part.Equals("S")) _startPos = pos;

                Firewall firewall = null;
                if (part[0].Equals('F'))
                {
                    firewall = ObjectManager.Instance.GetObject(ObjectID.Firewall).GetComponent<Firewall>();
                    part = part[1..];
                }

                Type tileType = part.Contains(",") ? typeof(SwapTile) : typeof(OperationTile);
                Tile tile = ObjectManager.Instance.GetObject(ObjectID.Tile).GetComponent(tileType) as Tile;
                tile!.enabled = true;
                tile!.transform.position = pos;
                tile!.Init(part);

                _tileMap.TryAdd(pos, tile);

                if (!firewall) continue;
                firewall.transform.position = pos;
                firewall.Init(pos);
                _firewalls.Add(pos, firewall);
            }
        }

        //Load Enemies
        foreach (EnemyInfo info in enemyInfo)
        {
            Enemy enemy = ObjectManager.Instance.GetObject(info.enemyType).GetComponent<Enemy>();
            enemy.transform.position = info.startPoint;
            enemy.Init(info.startPoint, info.endPoint);

            _enemies.Add(enemy);
        }
    }

    private void UnloadStage()
    {
        foreach (Enemy enemy in _enemies)
            enemy.gameObject.SetActive(false);

        foreach (KeyValuePair<Vector2, Firewall> firewall in _firewalls)
            firewall.Value.gameObject.SetActive(false);

        foreach (KeyValuePair<Vector2, Tile> tile in _tileMap)
            tile.Value.gameObject.SetActive(false);
    }

    public void Init()
    {
        Player player = GameManager.Instance.Player;
        player.Moves = moveCount;
        player.Value = startValue;
    }

    private void OnRestart()
    {
        Player player = GameManager.Instance.Player;
        player.Moves = moveCount;
        player.Value = startValue;
        player.transform.position = _startPos;
    }

    public T GetTile<T>(Vector2 pos) where T : Tile =>
        _tileMap.TryGetValue(pos, out Tile tile) ? tile as T : throw new ArgumentException();

    public bool TryGetTile<T>(Vector2 pos, out Tile tile) where T : Tile =>
        _tileMap.TryGetValue(pos, out tile);

    public Firewall GetFirewall(Vector2 pos) =>
        _firewalls.TryGetValue(pos, out Firewall firewall) ? firewall : throw new ArgumentException();

    public bool TryGetFirewall(Vector2 pos, out Firewall firewall) =>
        _firewalls.TryGetValue(pos, out firewall);

    #endregion
}

[Serializable]
public struct EnemyInfo
{
    public ObjectID enemyType;
    public Vector2 startPoint;
    public Vector2? endPoint;

    public EnemyInfo(ObjectID enemyType, Vector2 startPoint, Vector2? endPoint = null)
    {
        this.enemyType = enemyType;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }
}