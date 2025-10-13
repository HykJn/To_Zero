using System;
using UnityEngine;

public class Observer : Enemy
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    private bool _waiting;
    private Vector2 _destination;

    #endregion

    #region =====Unity Events=====

    private void OnEnable()
    {
        if (!GameManager.Instance || !GameManager.Instance.Player) return;
        GameManager.Instance.OnRestart += OnRestart;
        GameManager.Instance.Player.OnPlayerMove += Move;
    }

    private void OnDisable()
    {
        if (!GameManager.Instance || !GameManager.Instance.Player) return;
        GameManager.Instance.OnRestart -= OnRestart;
        GameManager.Instance.Player.OnPlayerMove -= Move;
    }

    #endregion

    #region =====Methods=====

    private void Move()
    {
        if ((Vector2)this.transform.position != _destination)
        {
            SwitchWarning(false);
            Vector2 dir = (_destination - (Vector2)this.transform.position).normalized;
            Vector2 pos = (Vector2)this.transform.position + dir;
            this.transform.position = pos;
        }
        else
        {
            if (_waiting)
            {
                _waiting = false;
                return;
            }

            SwitchWarning(false);
            _destination = _destination == startPoint ? endPoint : startPoint;
            _waiting = true;
        }

        SwitchWarning(true);
    }

    private void SwitchWarning(bool warning)
    {
        Stage stage = GameManager.Instance.Stage;
        Vector2 dir = (_destination - (Vector2)this.transform.position).normalized;
        Vector2 pos = (Vector2)this.transform.position + dir * 1.5f;
        for (float x = -0.5f; x <= 0.5f; x++)
        for (float y = -0.5f; y <= 0.5f; y++)
            if (stage.TryGetTile<OperationTile>(pos + new Vector2(x, y), out Tile tile))
                (tile as OperationTile)!.WarningCount += warning ? 1 : -1;

        if (!warning) return;

        for (float i = -0.5f; i <= 0.5f; i++)
        {
            if (dir == Vector2.up || dir == Vector2.down)
                pos = (Vector2)this.transform.position + dir + new Vector2(i, 0);
            else if (dir == Vector2.left || dir == Vector2.right)
                pos = (Vector2)this.transform.position + dir - new Vector2(0, i);

            if (!stage.TryGetFirewall(pos, out _)) continue;
            if (stage.TryGetTile<OperationTile>(pos + dir, out Tile tile))
                (tile as OperationTile)!.WarningCount--;
        }
    }

    private void OnHacked()
    {
    }

    public override void Init(Vector2 startPoint, Vector2 endPoint)
    {
        base.Init(startPoint, endPoint);
        OnRestart();
    }

    private void OnRestart()
    {
        // SwitchWarning(false);
        this.transform.position = startPoint;
        _destination = endPoint;
        _waiting = true;
        SwitchWarning(true);
    }

    #endregion
}