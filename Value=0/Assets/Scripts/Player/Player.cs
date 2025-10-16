using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GLOBAL;

public class Player : MonoBehaviour
{
    public event Action OnPlayerMove;

    //���� ��ź
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

    //����
    public bool IsMovable { get; set; } = true;
    public bool Controllable { get; set; } = true;

    #endregion

    #region =====Fields=====

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [SerializeField] private AnimationCurve easeOut;

    private Firewall _firewall;
    private bool _cube;


    private int _moves, _value;

    //���� �������� ��ź ������Ʈ
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

        //����
        if (GameManager.Instance.CurrentBossStage)
        {
            RegisterBossEvents();
        }
    }

    private void Update()
    {
        InputHandler();
    }

    //����
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
            else if (IsMovable)
            {
                Move((Vector2)this.transform.position + dir);
                if (dir == Vector2.right) spriteRenderer.flipX = true;
                else if (dir == Vector2.left) spriteRenderer.flipX = false;
            }
        }

        //Hold & Release firewall
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.Instance.CurrentBossStage)
            {
                // ���� �������� 
                ExplodeAllBombs();
            }
            else if (_cube)
            {
                _cube = false;
                if (Value != 0)
                {
                    Die();
                    SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
                }
                else
                {
                    if (SequanceManager.Stage == 6)
                    {
                        UIManager.Instance.DialogPanel.SetDialog(13);
                        UIManager.Instance.DialogPanel.StartDialog();
                    }
                    else if (SequanceManager.Stage == 12)
                    {
                        UIManager.Instance.DialogPanel.SetDialog(22);
                        UIManager.Instance.DialogPanel.StartDialog();
                    }
                    else if (SequanceManager.Stage == 16)
                    {
                        UIManager.Instance.DialogPanel.SetDialog(32);
                        UIManager.Instance.DialogPanel.StartDialog();
                    }
                }
            }
            else
            {
                if (!_firewall) return;
                if (!_firewall.IsHeld) HoldFirewall();
                else ReleaseFirewall();
            }
        }

        //Restart //����
        if (!GameManager.Instance.CurrentBossStage)
            if (Input.GetKeyDown(KeyCode.R) && IsMovable)
                GameManager.Instance.Restart();
    }

    public void Animation_SetMovable(int value)
    {
        if (value == 1) IsMovable = true;
        else IsMovable = false;
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
            }
            else
            {
                Die();
                SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
            }

            return;
        }

        IsMovable = false;
        StartCoroutine(Crtn_Move(pos));
    }

    private bool CheckIsMovable(Vector2 pos)
    {
        Stage stage = GameManager.Instance.Stage;
        if (_firewall) _firewall.IsSelected = false;
        _cube = false;
        if (stage.TryGetTile<Tile>(pos, out Tile t))
        {
            OperationTile tile = t as OperationTile;
            if (tile!.Operator == Operation.Cube)
            {
                _cube = true;
                return false;
            }
        }

        if (!stage.TryGetFirewall(pos, out Firewall firewall))
            return stage.TryGetTile<Tile>(pos, out _);

        _firewall = firewall;
        _firewall.IsSelected = true;

        return false;
    }

    //������������ �����߰�
    private IEnumerator Crtn_Move(Vector2 pos)
    {
        if (GameManager.Instance.Stage.TryGetTile<OperationTile>(this.transform.position, out Tile currentTile))
        {
            (currentTile as OperationTile).AnyObjectAbove = false;
        }

        Vector2 next = pos - (Vector2)this.transform.position;

        Vector2 startPos = this.transform.position;
        float t = 0f;
        animator.SetTrigger(Animator.StringToHash("Move"));

        if (_firewall) _firewall.IsSelected = false;
        _firewall = null;

        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerMove);
        while ((Vector2)this.transform.position != pos)
        {
            this.transform.position = Vector2.Lerp(startPos, pos, Mathf.Clamp01(easeOut.Evaluate(t / 0.05f)));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = pos;
        GameManager.Instance.Stage.GetTile<OperationTile>(this.transform.position).AnyObjectAbove = true;

        if (GameManager.Instance.Stage.TryGetFirewall((Vector2)this.transform.position + next, out Firewall firewall))
        {
            _firewall = firewall;
            _firewall.IsSelected = true;
        }
        else if (GameManager.Instance.Stage.TryGetTile<Tile>((Vector2)this.transform.position + next, out Tile temp))
        {
            _cube = (temp as OperationTile).Operator == Operation.Cube;
        }

        OperationTile tile = GameManager.Instance.Stage.GetTile<OperationTile>(pos);

        if (GameManager.Instance.CurrentBossStage)
        {
            // ���� ���������� Ÿ�� ó��
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
                    if (Value != tile.Value)
                    {
                        Die();
                        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
                        yield break;
                    }

                    break;
                case Operation.NotEqual:
                    if (Value == tile.Value)
                    {
                        Die();
                        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
                        yield break;
                    }

                    break;
                case Operation.Greater:
                    if (Value <= tile.Value)
                    {
                        Die();
                        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
                        yield break;
                    }

                    break;
                case Operation.Less:
                    if (Value >= tile.Value)
                    {
                        Die();
                        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
                        yield break;
                    }

                    break;
                case Operation.Portal:
                    if (Value != 0)
                    {
                        Die();
                        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerRespawn);
                    }
                    else GameManager.Instance.StageNumber++;

                    yield break;
            }
        }


        Moves--;
        OnPlayerMove?.Invoke();

        if (!GameManager.Instance.CurrentBossStage)
        {
            if (GameManager.Instance.Stage.GetTile<OperationTile>(pos).WarningCount > 0)
            {
                Die();
                SoundManager.Instance.PlayOneShot(SFX_ID.ObserverDetect);
                yield break;
            }
        }

        IsMovable = true;
    }

    private void HoldFirewall()
    {
        if (!_firewall) return;
        SoundManager.Instance.Play(SFX_ID.PlayerHoldBox);
        animator.SetTrigger(Animator.StringToHash("Hold"));
        _firewall.OnHeld();
        IsMovable = false;
    }

    private void ReleaseFirewall()
    {
        if (!_firewall) return;
        SoundManager.Instance.Play(SFX_ID.PlayerReleaseBox);
        animator.SetTrigger(Animator.StringToHash("Release"));
        _firewall.OnRelease();
        IsMovable = true;
    }

    private void MoveBox(Vector2 dir)
    {
        if (!_firewall.Move(dir)) return;

        Moves--;
        ReleaseFirewall();
        _firewall = null;
        OnPlayerMove?.Invoke();
    }

    private void OnRestart()
    {
        _firewall = null;
        _cube = false;
        IsMovable = true;

        //����
        if (GameManager.Instance.CurrentBossStage)
        {
            ClearBombs();
        }
    }

    private void Die()
    {
        IsMovable = false;
        animator.SetTrigger(Animator.StringToHash("Die"));
    }

    private void ApplyTileValueBoss(Vector3 pos)
    {
        OperationTile tile = GameManager.Instance.Stage.GetTile<OperationTile>(pos);
        if (!tile) return;

        // ���� �������������� ���� Ÿ�Ͽ� ��ź ����
        switch (tile.Operator)
        {
            case Operation.None:
            case Operation.Portal:
                // ��Ż�� Value�� 0�� ���� ���� ��������
                if (tile.Operator == Operation.Portal && Value == 0)
                {
                    // ���� �������� Ŭ���� ó��
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

        // �������� ���� Value ����
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