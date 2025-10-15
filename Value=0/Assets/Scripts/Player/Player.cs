using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GLOBAL;

public class Player : MonoBehaviour
{
    public event Action OnPlayerMove;
    //보스 폭탄
    public event Action<int> OnBombExplode;

    #region =====Properties=====

    public int Moves
    {
        get => _moves;
        set
        {
            _moves = value;
            UIManager.Instance.MatrixUI.Moves = _moves;
        }
    }

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            UIManager.Instance.MatrixUI.Value = _value;
        }
    }

    //보스
    public bool IsMovable { get; set; } = true;
    public bool Controllable { get; set; } = true;

    #endregion

    #region =====Fields=====

    [Header("Components")]
    [SerializeField] private Animator animator;

    //보스 게임오버 움직이면안됨
    public bool _isMovable = true;
    private Firewall _firewall;

    private int _moves, _value;

    //보스 스테이지 폭탄 오브젝트
    [SerializeField] private GameObject bombObj;
    private Dictionary<Vector3, GameObject> bombPositions = new Dictionary<Vector3, GameObject>();

    #endregion

    #region =====Unity Events=====

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRestart += OnRestart;
        }
        //보스
        if (GameManager.Instance.CurrentBossStage)
        {
            RegisterBossEvents();
        }
    }

    private void Update()
    {
        InputHandler();
    }

    //보스
    private void OnDisable()
    {
        if (GameManager.Instance?.CurrentBossStage == true)
        {
            UnregisterBossEvents();
        }
    }


    #endregion

    #region =====Methods=====

    private void InputHandler()
    {
        if (UIManager.Instance.AnyPanelOpen) return;
        //Move
        Vector2 dir = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2.right;
        if (dir != Vector2.zero)
        {
            if (_firewall && _firewall.IsHeld) MoveBox(dir);
            else if (_isMovable) Move((Vector2)this.transform.position + dir);
        }

        //Hold & Release firewall
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.Instance.CurrentBossStage)
            {
                // 보스 스테이지 
                ExplodeAllBombs();
            }
            else
            {
         
                if (!_firewall) return;
                if (!_firewall.IsHeld) HoldFirewall();
                else ReleaseFirewall();
            }
        }

        //Restart //보스
        if (!GameManager.Instance.CurrentBossStage)
            if (Input.GetKeyDown(KeyCode.R)) GameManager.Instance.Restart();
    }

    public void Animation_SetMovable(int value)
    {
        if (value == 1) _isMovable = true;
        else _isMovable = false;
    }

    private void Move(Vector2 pos)
    {
        if (!CheckIsMovable(pos)) return;

        if (Moves <= 0)
        {
            if (GameManager.Instance.CurrentBossStage)
            {
                //Die();
                ExplodeAllBombs();
                return;

            }
            else
            {
                Die();
                return;
            }
        }

        StartCoroutine(Crtn_Move(pos));
    }

    private bool CheckIsMovable(Vector2 pos)
    {
        Stage stage = GameManager.Instance.Stage;
        if (!stage.TryGetFirewall(pos, out Firewall firewall))
            return stage.TryGetTile<Tile>(pos, out _);

        if (_firewall) _firewall.IsSelected = false;
        _firewall = firewall;
        _firewall.IsSelected = true;

        return false;
    }

    //보스스테이지 조건추가
    private IEnumerator Crtn_Move(Vector2 pos)
    {
        if (GameManager.Instance.Stage.TryGetTile<OperationTile>(this.transform.position, out Tile currentTile))
        {
            (currentTile as OperationTile).AnyObjectAbove = false;
        }

        Vector2 next = pos - (Vector2)this.transform.position;

        this.transform.position = pos;
        if (_firewall) _firewall.IsSelected = false;
        GameManager.Instance.Stage.GetTile<OperationTile>(this.transform.position).AnyObjectAbove = true;

        if (GameManager.Instance.Stage.TryGetFirewall((Vector2)this.transform.position + next, out Firewall firewall))
        {
            _firewall = firewall;
            _firewall.IsSelected = true;
        }

        OperationTile tile = GameManager.Instance.Stage.GetTile<OperationTile>(pos);

        if (GameManager.Instance.CurrentBossStage)
        {
            // 보스 스테이지용 타일 처리
            ApplyTileValueBoss(pos);
        }
        else
        {
            switch (tile.Operator)
            {
                case Operation.None:
                    break;
                case Operation.Add:
                    Value += tile.Value;
                    break;
                case Operation.Subtract:
                    Value -= tile.Value;
                    break;
                case Operation.Multiply:
                    Value *= tile.Value;
                    break;
                case Operation.Divide:
                    Value /= tile.Value;
                    break;
                case Operation.Equal:
                    if (Value != tile.Value) Die();
                    break;
                case Operation.NotEqual:
                    if (Value == tile.Value) Die();
                    break;
                case Operation.Greater:
                    if (Value <= tile.Value) Die();
                    break;
                case Operation.Less:
                    if (Value >= tile.Value) Die();
                    break;
                case Operation.Portal:
                    if (Value != 0) Die();
                    else
                    {
                        GameManager.Instance.StageNumber++;
                        yield break;
                    }

                    break;
            }
        }

        Moves--;
        OnPlayerMove?.Invoke();

        if (!GameManager.Instance.CurrentBossStage)
        {
            if (GameManager.Instance.Stage.TryGetTile<OperationTile>(pos, out Tile checkTile))
            {
                if ((checkTile as OperationTile).WarningCount > 0)
                {
                    Die();
                }
            }
        }

        yield return null;
    }

    private void HoldFirewall()
    {
        if (!_firewall) return;
        _firewall.OnHeld();
        _isMovable = false;
    }

    private void ReleaseFirewall()
    {
        if (!_firewall) return;
        _firewall.OnRelease();
        _isMovable = true;
    }

    private void MoveBox(Vector2 dir)
    {
        if (!_firewall.Move(dir)) return;

        Moves--;
        ReleaseFirewall();
        OnPlayerMove?.Invoke();
    }

    private void OnRestart()
    {
        _firewall = null;
        _isMovable = true;

        //보스
        if (GameManager.Instance.CurrentBossStage)
        {
            ClearBombs();
        }
    }

    private void Die()
    {
        animator.SetTrigger(Animator.StringToHash("Die"));
    }

    private void ApplyTileValueBoss(Vector3 pos)
    {
        OperationTile tile = GameManager.Instance.Stage.GetTile<OperationTile>(pos);
        if (!tile) return;

        // 보스 스테이지에서는 연산 타일에 폭탄 생성
        switch (tile.Operator)
        {
            case Operation.None:
            case Operation.Portal:
                // 포탈은 Value가 0일 때만 다음 스테이지
                if (tile.Operator == Operation.Portal && Value == 0)
                {
                    // 보스 스테이지 클리어 처리
                    GameManager.Instance.StageNumber++;
                }
                break;
            case Operation.Add:
                if (!bombPositions.ContainsKey(pos))
                {
                    CheckAndSpawnBomb(bombObj, pos);
                    Value += tile.Value;
                }
                break;
            case Operation.Subtract:
                if (!bombPositions.ContainsKey(pos))
                {
                    CheckAndSpawnBomb(bombObj, pos);
                    Value -= tile.Value;
                }
                break;
            case Operation.Multiply:
                if (!bombPositions.ContainsKey(pos))
                {
                    CheckAndSpawnBomb(bombObj, pos);
                    Value *= tile.Value;
                }
                break;
            case Operation.Divide:
                if (!bombPositions.ContainsKey(pos))
                {
                    CheckAndSpawnBomb(bombObj, pos);
                    Value /= tile.Value;
                }
                break;
            case Operation.Equal:
            case Operation.NotEqual:
            case Operation.Greater:
            case Operation.Less:
                
                break;
        }
    }

    private void CheckAndSpawnBomb(GameObject bomb, Vector3 pos)
    {
        if (bombPositions.ContainsKey(pos)) return;

        GameObject newBomb = Instantiate(bomb, pos, Quaternion.identity);
        bombPositions.Add(pos, newBomb);
    }

    private void ExplodeAllBombs()
    {
        if (bombPositions.Count == 0) return;

        foreach (var bomb in bombPositions.Values)
        {
            if (bomb != null) Destroy(bomb);
        }
        bombPositions.Clear();

        // 보스에게 현재 Value 전달
        OnBombExplode?.Invoke(Value);
    }

    public void ClearBombs()
    {
        foreach (var bomb in bombPositions.Values)
        {
            if (bomb != null) Destroy(bomb);
        }
        bombPositions.Clear();
    }

    public void RegisterBossEvents()
    {
        if (BossManager.Instance != null)
        {
            BossManager.Instance.OnPlayerLose += HandleGameOver;
            BossManager.Instance.OnPlayerWin += HandleGameOver;
        }
    }

    public void UnregisterBossEvents()
    {
        if (BossManager.Instance != null)
        {
            BossManager.Instance.OnPlayerLose -= HandleGameOver;
            BossManager.Instance.OnPlayerWin -= HandleGameOver;
        }
    }

    private void HandleGameOver()
    {
        IsMovable = false;
        Controllable = false;
    }

    #endregion
}