using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GLOBAL;

public class Stage : MonoBehaviour
{
    //���� �������� �� ����  event

    #region ==========Events==========

    #endregion

    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("Configuration")]
    [SerializeField] private int moveCount;
    [SerializeField] private int startValue;
    [SerializeField, TextArea(5, 20)] private string tileInfo;
    [SerializeField] private EnemyInfo[] enemyInfo;

    private Dictionary<Vector2, Tile> _tileMap;
    private List<Firewall> _firewalls;
    private List<Enemy> _enemies;
    private Vector2 _startPos;

    #endregion

    #region =====Unity Events=====

    private void OnEnable()
    {
        LoadStage();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRestart += OnRestart;
        }
    }

    private void OnDisable()
    {
        UnloadStage();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRestart -= OnRestart;
        }
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
                    firewall.gameObject.SetActive(false);
                }

                Tile tile = ObjectManager.Instance
                    .GetObject(part.Contains(',') ? ObjectID.SwapTile : ObjectID.OperationTile)
                    .GetComponent<Tile>();
                tile!.transform.position = pos;
                tile!.Init(part);

                _tileMap.TryAdd(pos, tile);

                if (!firewall) continue;
                firewall.transform.position = pos;
                firewall.gameObject.SetActive(true);
                firewall.Init(pos);
                _firewalls.Add(firewall);
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
        if (_enemies != null)
        {
            foreach (Enemy enemy in _enemies)
            {
                if (enemy != null && enemy.gameObject != null)
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }

        if (_firewalls != null)
        {
            foreach (Firewall firewall in _firewalls)
            {
                if (firewall != null && firewall.gameObject != null)
                {
                    firewall.gameObject.SetActive(false);
                }
            }
        }

        if (_tileMap != null)
        {
            foreach (var kvp in _tileMap.ToList())
            {
                if (kvp.Value != null && kvp.Value.gameObject != null)
                {
                    kvp.Value.gameObject.SetActive(false);
                }
            }
        }

        _tileMap = null;
        _firewalls = null;
        _enemies = null;
    }

    public void Init()
    {
        Player player = GameManager.Instance.Player;

        player.IsMovable = true;
        player.Moves = moveCount;
        player.Value = startValue;
        player.transform.position = _startPos;

        UIManager.Instance.MatrixUI.Moves = moveCount;
        UIManager.Instance.MatrixUI.Value = startValue;

        if (_tileMap != null && _tileMap.ContainsKey(_startPos))
        {
            GetTile<OperationTile>(_startPos).AnyObjectAbove = true;
        }
    }

    private void OnRestart() => Init();

    public T GetTile<T>(Vector2 pos) where T : Tile =>
        _tileMap.TryGetValue(pos, out Tile tile) ? tile as T : throw new ArgumentException();

    public bool TryGetTile<T>(Vector2 pos, out Tile tile) where T : Tile =>
        _tileMap.TryGetValue(pos, out tile);

    public Firewall GetFirewall(Vector2 pos) =>
        _firewalls.FirstOrDefault(firewall => firewall.Position == pos);

    public bool TryGetFirewall(Vector2 pos, out Firewall firewall)
    {
        firewall = GetFirewall(pos);
        return firewall;
    }

    #endregion
}

[Serializable]
public struct EnemyInfo
{
    public ObjectID enemyType;
    public Vector2 startPoint;
    public Vector2 endPoint;

    public EnemyInfo(ObjectID enemyType, Vector2 startPoint, Vector2 endPoint)
    {
        this.enemyType = enemyType;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }
}