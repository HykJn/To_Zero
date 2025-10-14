using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GLOBAL;

public class Player : MonoBehaviour
{
    public event Action OnPlayerMove;

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

    #endregion

    #region =====Fields=====

    [Header("Components")]
    [SerializeField] private Animator animator;

    private bool _isMovable = true;
    private Firewall _firewall;

    private int _moves, _value;

    #endregion

    #region =====Unity Events=====

    private void Start()
    {
        GameManager.Instance.OnRestart += OnRestart;
    }

    private void Update()
    {
        InputHandler();
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
            if (!_firewall) return;
            if (!_firewall.IsHeld) HoldFirewall();
            else ReleaseFirewall();
        }

        //Restart
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
            Die();
            return;
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

    private IEnumerator Crtn_Move(Vector2 pos)
    {
        GameManager.Instance.Stage.GetTile<OperationTile>(this.transform.position).AnyObjectAbove = false;
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

        Moves--;
        OnPlayerMove?.Invoke();

        if (GameManager.Instance.Stage.GetTile<OperationTile>(pos).WarningCount > 0) Die();

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
    }

    private void Die()
    {
        animator.SetTrigger(Animator.StringToHash("Die"));
    }

    #endregion
}