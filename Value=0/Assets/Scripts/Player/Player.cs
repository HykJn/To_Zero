using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ==========Events==========

    public event Action OnPlayerMove;

    #endregion

    #region ==========Properties==========

    public int Value { get; private set; }
    public int Moves { get; private set; }
    public bool IsMovable { get; set; } = true;
    public bool Controllable { get; set; } = true;

    #endregion

    #region ==========Fields==========

    [SerializeField] private AnimationCurve easeOutCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col;

    private Box _box;
    private bool _onHold = false;

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
            if (_onHold) UnHoldBox();
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
        if (Moves <= 0)
        {
            Die(EventID.PlayerDieByMoves);
            return;
        }

        if (CheckBox(this.transform.position + dir) ||
            !GameManager.Instance.CurrentStage.IsMovable(this.transform.position + dir)) return;

        GameManager.Instance.CurrentStage.GetTile(this.transform.position).OnPlayer = false;

        col.enabled = false;
        StartCoroutine(Crtn_Move(this.transform.position, this.transform.position + dir));
    }

    private IEnumerator Crtn_Move(Vector3 from, Vector3 to)
    {
        SoundManager.Instance.PlayOneShot_SFX(SFXID.PlayerMove);
        animator.SetTrigger(Animator.StringToHash("Move"));

        IsMovable = false;
        float t = 0;
        while (Vector3.Distance(this.transform.position, to) >= 0.05f)
        {
            this.transform.position = Vector3.Lerp(from, to, easeOutCurve.Evaluate(t / 0.06f));
            t += Time.deltaTime;
            yield return null;
        }

        this.transform.position = to;
        IsMovable = true;

        CheckTile();
        CheckBox(to + (to - from).normalized);
        col.enabled = true;
    }

    private void HoldBox()
    {
        if (!_box) return;
        SoundManager.Instance.Play_SFX(SFXID.PlayerHoldBox);
        IsMovable = false;

        _box.SetPreview(true);

        _onHold = true;
        _box.Selected = true;

        animator.SetBool(Animator.StringToHash("HoldBox"), true);
    }

    private void UnHoldBox()
    {
        SoundManager.Instance.Play_SFX(SFXID.PlayerUnholdBox);
        _onHold = false;
        _box.Selected = false;
        if (_box) _box.SetPreview(false);

        animator.SetBool(Animator.StringToHash("HoldBox"), false);
    }

    private void MoveBox()
    {
        if (!_onHold) return;

        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;

        if (dir != Vector3.zero) _box.Move(dir);
        UnHoldBox();
        _box = null;

        Moves--;
        OnPlayerMove?.Invoke();


        animator.SetTrigger(Animator.StringToHash("MoveBox"));
        animator.SetBool(Animator.StringToHash("HoldBox"), false);
    }

    private void CheckTile()
    {
        OperationTile tile = GameManager.Instance.CurrentStage.GetTile(this.transform.position);
        switch (tile.TileType)
        {
            case TileType.Portal:
                if (Value == 0) GameManager.Instance.Transition(EventID.NextStage);
                else Die(EventID.PlayerDieByMoves);
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
        }

        Moves--;
        OnPlayerMove?.Invoke();
        tile.OnPlayer = true;
    }

    private bool CheckBox(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos + Vector3.back, Vector3.forward, 5f, LayerMask.GetMask("Box"));
        if (hit)
        {
            if (_box) _box.Selected = false;
            _box = hit.collider.GetComponentInParent<Box>();
            _box.Selected = true;
            return true;
        }

        if (_box) _box.Selected = false;
        _box = null;
        return false;
    }

    public void Die(EventID diedBy)
    {
        GameManager.Instance.Transition(diedBy);
    }

    private void Restart()
    {
        if (_onHold && !IsMovable)
        {
            _onHold = false;
            UnHoldBox();
            if (_box) _box.SetPreview(false);
            _box = null;
            animator.SetBool(Animator.StringToHash("HoldBox"), false);
        }
        else if (!IsMovable) return;

        GameManager.Instance.Restart();
    }

    #endregion

    #region ==========For Animation Event==========

    public void AnimationMoveLock()
    {
        IsMovable = !IsMovable;
    }

    #endregion
}