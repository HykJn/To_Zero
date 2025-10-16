using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GLOBAL;


public class BossManager : MonoBehaviour
{


    #region ==========Events==========

    public event Action<int> PlayerHealthChange;
    public event Action<int> BossHealthChange;
    //public event Action<int> BossTargetValueChange;
    public event Action OnPlayerWin;
    public event Action OnPlayerLose;
    public event Action<int> OnPhaseChange;

    #endregion

    #region ==========Properties==========

    public static BossManager Instance { get; private set; }
    public int BossTargetValue { get; private set; }
    public int CurrentPhase { get; private set; } = 1;

    public int PlayerHealth
    {
        get => _playerHealth;
        private set
        {
            _playerHealth = value;
            PlayerHealthChange?.Invoke(_playerHealth);

            if (_playerHealth <= 0)
            {
                OnPlayerLose?.Invoke();
            }
        }
    }

    public int BossHealth
    {
        get => _bossHealth;
        private set
        {
            _bossHealth = value;
            BossHealthChange?.Invoke(_bossHealth);

            if (_bossHealth <= 8 && CurrentPhase == 1)
            {
                ChangeToPhase2();
            }

            if (_bossHealth <= 0)
            {
                OnPlayerWin?.Invoke();
            }
        }
    }


    #endregion

    #region ==========Fields==========

    [SerializeField] private int initPlayerHealth = 3;
    [SerializeField] private int initBossHealth = 12;

    [Header("1페이즈 패턴")]
    [SerializeField] private int phase1MoveInterval = 3;

    private int _playerHealth;
    private int _bossHealth;
    private bool _isBombSubscribed = false;
    private int _playerMoveCount = 0;
    private List<BossLaser> _currentLasers;
    private bool _isPhaseChanging = false;
    public bool _isBossStagePlay = false;

    [SerializeField] private GameObject bossObject;
    [SerializeField] private BossHitEffect bossHitEffect;

    [SerializeField] private List<GameObject> bosshealthObj = new List<GameObject>();
    private int bosshealthObjCount = 4;

    [SerializeField] private List<GameObject> playerhealthObj = new List<GameObject>();
    private int playerhealthObjCount = 3;

    [SerializeField] private TMP_Text text_BossTartgetValue;


    #endregion



    #region ==========Unity Methods==========

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region ==========Methods==========

    public void InitBossBattle(int bossTargetValue)
    {

        bossObject.SetActive(true);
        UpdateTargetText(bossTargetValue);

        bosshealthObjCount = 4;
        foreach (var BosshealthObj in bosshealthObj)
        {
            BosshealthObj.SetActive(false);
        }

        playerhealthObjCount = 3;
        foreach (var PlayerhealthObj in playerhealthObj)
        {
            PlayerhealthObj.SetActive(true);
        }

        GameManager.Instance.Player?.ClearBombs();
        if (!_isBombSubscribed && GameManager.Instance?.Player != null)
        {
            GameManager.Instance.Player.OnBombExplode += HandleBombExplod;
            GameManager.Instance.Player.OnPlayerMove += OnPlayerMove;
            _isBombSubscribed = true;
        }

        _playerHealth = initPlayerHealth;
        _bossHealth = initBossHealth;
        BossTargetValue = bossTargetValue;
        CurrentPhase = 1;
        _playerMoveCount = 0;

        PlayerHealthChange?.Invoke(_playerHealth);
        BossHealthChange?.Invoke(_bossHealth);
        //BossTargetValueChange?.Invoke(BossTargetValue);

        GameManager.Instance.Player?.RegisterBossEvents();

        Debug.Log("보스 전투시작 1페이즈");
    }

    public void UpdateBossTargetValue(int bossTargetValue)
    {
        BossTargetValue = bossTargetValue;
        //BossTargetValueChange?.Invoke(BossTargetValue);
        UpdateTargetText(bossTargetValue);
        Debug.Log($"보스 목표값 업데이트: {BossTargetValue}");
    }

    private void UpdateTargetText(int value)
    {
        if (value > 0)
            text_BossTartgetValue.text = $"+{value}";
        else
            text_BossTartgetValue.text = $"{value}";
    }

    public void CheckBombResult(int playerFinalValue)
    {
        if (playerFinalValue == BossTargetValue)
        {
            BossHealth--;


            bosshealthObj[bosshealthObjCount - 1].SetActive(true);
            bosshealthObjCount--;
            if (bosshealthObjCount == 0 && !(BossHealth == 0))
            {
                bosshealthObjCount = 4;
                foreach (var healthObj in bosshealthObj)
                {
                    healthObj.SetActive(false);
                }
            }

            bossHitEffect.PlayerHitEffect();
            Debug.Log($"승리! 보스 체력: {BossHealth}");
        }
        else
        {
            PlayerHealth--;
            playerhealthObj[playerhealthObjCount - 1].SetActive(false);
            playerhealthObjCount--;
            Debug.Log($"패배! 플레이어 체력: {PlayerHealth}");
        }
    }


    private void HandleBombExplod(int playerFinalValue)
    {
        CheckBombResult(playerFinalValue);

        if (_bossHealth <= 0 || _playerHealth <= 0)
        {
            EndBossBattle();
            return;
        }

        if (_bossHealth > 0 && _playerHealth > 0)
        {

            if (_isPhaseChanging)
            {
                _isPhaseChanging = false;
            }
            else
            {
                GameManager.Instance.Stage.LoadNextBossStage();
            }
            _playerMoveCount = 0;
            ClearLaser();
        }
    }

    private void EndBossBattle()
    {
        if (_isBombSubscribed && GameManager.Instance?.Player != null)
        {
            GameManager.Instance.Player.OnBombExplode -= HandleBombExplod;
            GameManager.Instance.Player.OnPlayerMove -= OnPlayerMove;
            _isBombSubscribed = false;

        }
        ClearLaser();
        GameManager.Instance.Player?.UnregisterBossEvents();
        bossObject.SetActive(false);
    }

    private void ChangeToPhase2()
    {
        _isPhaseChanging = true;
        CurrentPhase = 2;
        OnPhaseChange?.Invoke(CurrentPhase);
        ClearLaser();
        _playerMoveCount = 0;
        GameManager.Instance.Stage.LoadBossStageByPhase(2);
    }

    private void OnPlayerMove()
    {
        if (CurrentPhase == 1)
        {
            _playerMoveCount++;
            if (_playerMoveCount == 1)
            {
                ClearLaser();
            }
            else if (_playerMoveCount == 2)
            {
                ShowLaserWarning();
            }
            else if (_playerMoveCount >= phase1MoveInterval)
            {
                _playerMoveCount = 0;
                Phase1LaserAttack();
            }
        }
    }
    private void ShowLaserWarning()
    {

        List<Vector3> tiles = GameManager.Instance.Stage.GetAllTilePosition();

        if (tiles.Count == 0) return;

        Vector3 baseTile = tiles[UnityEngine.Random.Range(0, tiles.Count)];
        bool isRow = UnityEngine.Random.Range(0, 2) == 0;

        List<Vector3> attackPosition;
        if (isRow)
        {
            attackPosition = GameManager.Instance.Stage.GetRowPosition(baseTile);
        }
        else
        {
            attackPosition = GameManager.Instance.Stage.GetColumePositions(baseTile);
        }
        if (attackPosition.Count == 0) return;

        _currentLasers = new List<BossLaser>();

        foreach (Vector3 pos in attackPosition)
        {
            GameObject laserObj = ObjectManager.Instance.GetObject(ObjectID.BossLaser);
            BossLaser laser = laserObj.GetComponent<BossLaser>();
            laser.ShowWarning(pos);
            _currentLasers.Add(laser);

        }
    }

    private void Phase1LaserAttack()
    {
        if (_currentLasers == null) return;

        Vector3 playerPos = GameManager.Instance.Player.transform.position;
        bool isHit = false;

        foreach (BossLaser laser in _currentLasers)
        {
            Vector3 laserPos = laser.transform.position;
            if (Vector3.Distance(playerPos, laserPos) < 0.1f)
            {
                isHit = true;
                break;
            }
        }
        if (isHit)
        {
            DamagePlayer();
        }
        foreach (BossLaser laser in _currentLasers)
        {
            if (laser != null)
            {
                laser.Attack();
            }
        }
        _currentLasers.Clear();
        _currentLasers = null;
    }

    public void DamagePlayer()
    {
        PlayerHealth--;
        playerhealthObj[playerhealthObjCount - 1].SetActive(false);
        playerhealthObjCount--;
    }

    private void ClearLaser()
    {
        if (_currentLasers == null) return;

        foreach (BossLaser laser in _currentLasers)
        {
            ObjectManager.Instance.ReleaseObject(laser.gameObject);
        }

        _currentLasers.Clear();
        _currentLasers = null;
    }
    #endregion

}
