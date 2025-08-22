using System;
using System.Collections;
using UnityEngine;
using static GlobalDefines;

public class Player : MonoBehaviour
{
    #region ==========Events==========

    public event Action OnPlayerMove;

    #endregion

    #region ==========Properties==========

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            UIManager.Instance.InGameUI.Value = value;
        }
    }

    public int Moves
    {
        get => _moves;
        set
        {
            _moves = value;
            UIManager.Instance.InGameUI.Moves = value;
        }
    }

    public bool IsMovable { get; set; } = true;
    public bool Controllable { get; set; } = true;

    #endregion

    #region ==========Fields==========

    [SerializeField] private AnimationCurve easeOutCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col;

    private Box _box;
    private bool _onHold;
    private int _value;
    private int _moves;

    #endregion

    #region ==========Unity==========

    private void Update()
    {
        InputHandler();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Scanner"))
        {
            Die(EventID.PlayerDieByDrone);
        }
    }

    #endregion

    #region ==========Methods==========

    private void InputHandler()
    {
        if (UIManager.Instance.AnyPanelActivated || !Controllable) return;
        //Mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //TODO; Do something
        }

        //Moves
        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A))
        {
            dir = Vector3.left;
            if (!_onHold) spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D))
        {
            dir = Vector3.right;
            if (!_onHold) spriteRenderer.flipX = false;
        }

        if (dir != Vector3Int.zero)
        {
            Move(dir);
        }

        //Move Box
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_onHold) ReleaseBox();
            else HoldBox();
        }

        if (_onHold)
        {
            MoveBox();
        }

        //Restart
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    private void Move(Vector3 dir)
    {
        if (!IsMovable) return;

        Vector3 to = this.transform.position + dir;
        if (TryGetBoxAt(to)) return;
        if (!IsMovableTo(to)) return;

        StartCoroutine(OnMove(this.transform.position, to));
    }

    private IEnumerator OnMove(Vector3 from, Vector3 to)
    {
        IsMovable = false;
        float t = 0;

        animator.SetTrigger(Animator.StringToHash("Move"));
        SoundManager.Instance.PlayOneShot_SFX(SFX_ID.PlayerMove);

        while (Vector3.Distance(this.transform.position, to) >= 0.05f)
        {
            this.transform.position = Vector3.Lerp(from, to, easeOutCurve.Evaluate(t / 0.06f));
            t += Time.deltaTime;
            yield return null;
        }

        this.transform.position = to;
        IsMovable = true;

        ApplyTileValueAt(to);
        TryGetBoxAt(to + (to - from).normalized);

        Moves--;
        OnPlayerMove?.Invoke();

        col.enabled = true;
    }

    private void ApplyTileValueAt(Vector3 pos)
    {
        Tile tile = GameManager.Instance.Stage.GetTile(pos);
        if (!tile) return;

        switch (tile.TileType)
        {
            case TileType.None or TileType.Start:
                break;
            case TileType.Portal:
                CameraEffector.Instance.Transition(Value == 0 ? EventID.NextStage : EventID.PlayerDieByMoves);
                break;
            case TileType.Add:
                Value += tile.Value;
                break;
            case TileType.Sub:
                Value -= tile.Value;
                break;
            case TileType.Mul:
                Value *= tile.Value;
                break;
            case TileType.Div:
                Value /= tile.Value;
                break;
            case TileType.Equal:
                if (Value != tile.Value) Die(EventID.PlayerDieByMoves);
                break;
            case TileType.Not:
                if (Value == tile.Value) Die(EventID.PlayerDieByMoves);
                break;
            case TileType.Greater:
                if (Value <= tile.Value) Die(EventID.PlayerDieByMoves);
                break;
            case TileType.Less:
                if (Value >= tile.Value) Die(EventID.PlayerDieByMoves);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool IsMovableTo(Vector3 pos)
    {
        Tile tile = GameManager.Instance.Stage.GetTile(pos);
        if (!tile) return false;
        return !GameManager.Instance.Stage.GetBox(pos) &&
               tile.TileType != TileType.None;
    }

    private bool TryGetBoxAt(Vector3 pos)
    {
        if (_box) _box.IsSelected = false;
        _box = GameManager.Instance.Stage.GetBox(pos);
        if (_box) _box.IsSelected = true;
        return _box;
    }

    private void HoldBox()
    {
        if (!_box) return;

        IsMovable = false;
        _onHold = true;
        _box.OnHold(this.transform.position);

        animator.SetBool(Animator.StringToHash("HoldBox"), true);
        SoundManager.Instance.Play_SFX(SFX_ID.PlayerHoldBox);
    }

    private void ReleaseBox()
    {
        IsMovable = true;
        _onHold = false;
        if (_box) _box.Release();

        animator.SetBool(Animator.StringToHash("HoldBox"), false);
        SoundManager.Instance.Play_SFX(SFX_ID.PlayerReleaseBox);
    }

    private void MoveBox()
    {
        if (!_onHold) return;

        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;
        if (dir == Vector3.zero) return;

        if (!_box.Move(dir, this.transform.position)) return;
        ReleaseBox();
        _box.IsSelected = false;
        _box = null;

        Moves--;
        OnPlayerMove?.Invoke();

        animator.SetTrigger(Animator.StringToHash("MoveBox"));
        animator.SetBool(Animator.StringToHash("HoldBox"), false);
    }

    public void Die(EventID diedBy)
    {
        CameraEffector.Instance.Transition(diedBy);
    }

    private void Restart()
    {
        if (_onHold && !IsMovable)
        {
            ReleaseBox();
            _box.IsSelected = false;
            _box = null;
            animator.SetBool(Animator.StringToHash("HoldBox"), false);
        }
        else if (!IsMovable) return;

        _box = null;
        GameManager.Instance.Restart();
    }

    #endregion

    #region ==========For Animation Event==========

    public void AnimationMoveLock(int isLock)
    {
        IsMovable = isLock != 1;
    }

    #endregion
}