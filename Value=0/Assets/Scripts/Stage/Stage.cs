using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GLOBAL;

public class Stage : MonoBehaviour
{
    //���� �������� �� ����  event
    #region ==========Events==========
    public event Action<int, int, Vector3> OnBossStageLoaded;
    #endregion

    #region =====Properties=====
    //���� ��������
    public int StartValue => GameManager.Instance.CurrentBossStage ? bossStageMaps[currentBossMapIndex].startValue : startValue;
    public int MoveCount => GameManager.Instance.CurrentBossStage ? bossStageMaps[currentBossMapIndex].moveCount : moveCount;
    public Vector3 StartPos => _startPos;
    #endregion

    #region =====Fields=====

    [Header("Configuration")]
    [SerializeField] private int moveCount;
    [SerializeField] private int startValue;
    [SerializeField, TextArea(5, 20)] private string tileInfo;
    [SerializeField] private EnemyInfo[] enemyInfo;

    //���� ��������
    [Header("Boss Stage Configuration")]
    [SerializeField] private BossStageMap[] bossStageMaps;
    private int currentBossMapIndex = 0;
    private int currentPhase = 1;
    private List<int> _phase1Map;
    private List<int> _phase2Map;
    private int _currentPhase1Index = 0;
    private int _currentPhase2Index = 0;

    [Header("Stage Position Offset")]
    [SerializeField] private float normalStageOffset = 0f;
    [SerializeField] private float bossStageOffset = -2f;

    private Dictionary<Vector2, Tile> _tileMap;
    private List<Firewall> _firewalls;
    private List<Enemy> _enemies;
    private Vector2 _startPos;



    #endregion

    #region =====Unity Events=====

    private void OnEnable()
    {
        if (_tileMap == null || _tileMap.Count == 0)
        {
            LoadStage();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRestart += OnRestart;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance?.CurrentBossStage != true)
        {
            UnloadStage();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRestart -= OnRestart;
        }
    }

    #endregion

    #region =====Methods=====

    private void LoadStage()
    {
        if (GameManager.Instance.CurrentBossStage)
        {
            LoadBossStage();
            return;
        }

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
    //���� �������� NULL �߰�
    private void UnloadStage()
    {
        if (GameManager.Instance?.CurrentBossStage == true)
        {
            return;
        }

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

        if (GameManager.Instance.CurrentBossStage)
        {
            // ���� ���������� OnBossStageLoaded �̺�Ʈ���� �ʱ�ȭ��
            return;
        }

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

    //���� �������� �޼��� �߰�
    public void LoadBossStage()
    {
    
        _phase1Map = new List<int>();
        for (int i = 0; i < bossStageMaps.Length; i++)
        {
            if (bossStageMaps[i].phase == 1)
            {
                _phase1Map.Add(i);
            }
        }
        ShuffleBossMaps(_phase1Map);

        _phase2Map = new List<int>();
        for (int i = 0; i < bossStageMaps.Length; i++)
        {
            if (bossStageMaps[i].phase == 2)
            {
                _phase2Map.Add(i);
            }
        }
        ShuffleBossMaps(_phase2Map);


        _currentPhase1Index = 0;
        _currentPhase2Index = 0;
        currentPhase = 1;

        currentBossMapIndex = _phase1Map[0];
        //currentBossMapIndex = _phase1Map[_currentPhase1Index];

        if (bossStageMaps == null || bossStageMaps.Length == 0) return;

        BossStageMap currentStage = bossStageMaps[currentBossMapIndex];
        InitStage(currentStage.stageMap);

        OnBossStageLoaded?.Invoke(currentStage.startValue, currentStage.moveCount, StartPos);
        if (BossManager.Instance != null)
        {
            BossManager.Instance.InitBossBattle(currentStage.bossTargetValue);
        }

        Debug.Log($"���� �������� �ε� �ε���: {currentBossMapIndex}");

    }

    public void LoadNextBossStage()
    {
        if (!GameManager.Instance.CurrentBossStage) return;

        if (currentPhase == 1)
        {
            _currentPhase1Index++;
            currentBossMapIndex = _phase1Map[_currentPhase1Index];
        }
        else if (currentPhase == 2)
        {
            _currentPhase2Index++;
            currentBossMapIndex = _phase2Map[_currentPhase2Index];
        }

        BossStageMap nextStage = bossStageMaps[currentBossMapIndex];

        if (nextStage.phase != currentPhase)
        {
            currentPhase = nextStage.phase;
        }

        ClearStage();
        InitStage(nextStage.stageMap);
        InitTilesAndDrones();

        OnBossStageLoaded?.Invoke(nextStage.startValue, nextStage.moveCount, StartPos);
        if (BossManager.Instance != null)
        {
            BossManager.Instance.UpdateBossTargetValue(nextStage.bossTargetValue);
        }
    }

    private void ShuffleBossMaps(List<int> maps)
    {
        for (int i = maps.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = maps[i];
            maps[i] = maps[randomIndex];
            maps[randomIndex] = temp;
        }
    }

    public void LoadBossStageByPhase(int phase)
    {
        if (!GameManager.Instance.CurrentBossStage) return;
        currentPhase = phase;

        if (phase == 2)
        {
            _currentPhase2Index = 0;
            currentBossMapIndex = _phase2Map[_currentPhase2Index];
        }
        else
        {
            _currentPhase1Index = 0;
            currentBossMapIndex = _phase1Map[_currentPhase1Index];
        }

        BossStageMap phaseStage = bossStageMaps[currentBossMapIndex];

        ClearStage();
        InitStage(phaseStage.stageMap);
        InitTilesAndDrones();

        OnBossStageLoaded?.Invoke(phaseStage.startValue, phaseStage.moveCount, StartPos);
    }

    private void InitStage(string stageMapData)
    {
       
        _tileMap = new Dictionary<Vector2, Tile>();
        _firewalls = new List<Firewall>();  
        _enemies = new List<Enemy>();        
        //_objs = new List<GameObject>();

        string[] lines = stageMapData.Split('\n');
        //����
        lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

        int width = lines[0].Split(' ').Length;
        int height = lines.Length;

        float x = -(width / 2) + (width % 2 == 0 ? 0.5f : 0);
        float y = (height / 2) - (height % 2 == 0 ? 0.5f : 0);

        // ���� �������� ������ ����
        y += bossStageOffset;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Trim().Split(' ');
            for (int j = 0; j < parts.Length; j++)
            {
                string part = parts[j];
                Vector2 pos = new Vector2(x + j, y - i);

                if (part.Equals("0")) continue;
                if (part.Equals("S")) _startPos = pos;

                // Ÿ�� ����
                Tile tile = ObjectManager.Instance
                    .GetObject(part.Contains(',') ? ObjectID.SwapTile : ObjectID.OperationTile)
                    .GetComponent<Tile>();

                if (tile != null)
                {
                    tile.transform.position = pos;
                    tile.Init(part);
                    _tileMap.TryAdd(pos, tile);
                }
            }
        }
    }

    private void InitTilesAndDrones()
    {
        foreach (Tile tile in _tileMap.Values)
        {
            if (tile != null) tile.Init();
        }
    }

    public void ClearStage()
    {
        foreach (Tile tile in _tileMap.Values)
        {
            if (tile != null) ObjectManager.Instance.ReleaseObject(tile.gameObject);
        }
        _tileMap.Clear();
    }

    public List<Vector3> GetAllTilePosition()
    {
        return new List<Vector3>(_tileMap.Keys.Select(v => (Vector3)v));
    }

    public List<Vector3> GetColumePositions(Vector3 baseTile)
    {
        List<Vector3> columePosition = new List<Vector3>();

        foreach (Vector3 pos in _tileMap.Keys)
        {
            if (Mathf.Approximately(pos.x, baseTile.x))
            {
                columePosition.Add(pos);
            }
        }
        return columePosition;
    }

    public List<Vector3> GetRowPosition(Vector3 baseTile)
    {
        List<Vector3> rowPosition = new List<Vector3>();

        foreach (Vector3 pos in _tileMap.Keys)
        {
            if (Mathf.Approximately(pos.y, baseTile.y))
            {
                rowPosition.Add(pos);
            }

        }
        return rowPosition;
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

