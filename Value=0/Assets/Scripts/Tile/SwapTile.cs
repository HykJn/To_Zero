using System;
using UnityEngine;

public class SwapTile : OperationTile
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========

    private bool _swap = false;
    private readonly int[] _values = new int[2];
    private readonly TileType[] _opers = new TileType[2];

    #endregion

    #region ==========Unity Methods==========

    private void OnEnable()
    {
        GameManager.Instance.OnRestart += Init;
        GameObject.FindWithTag("Player").GetComponent<Player>().OnPlayerMove += Swap;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRestart -= Init;
        GameObject.FindWithTag("Player").GetComponent<Player>().OnPlayerMove -= Swap;
    }

    #endregion

    #region ==========Methods==========

    public void SetTile(string values)
    {
        string[] parts = values.Split(",");
        for (int i = 0; i < 2; i++)
        {
            _opers[i] = parts[i][0] switch
            {
                '+' => TileType.Add,
                '-' => TileType.Sub,
                '*' => TileType.Mul,
                '/' => TileType.Div,
                '=' => TileType.Equal,
                '!' => TileType.Not,
                '>' => TileType.Greater,
                '<' => TileType.Less,
                _ => throw new InvalidOperationException("Invalid operator in swap tile.")
            };
            this._values[i] = int.Parse(parts[i][1..]);
        }

        Init();
    }

    private void Swap()
    {
        _swap = !_swap;
        this.TileType = _swap ? _opers[1] : _opers[0];
        this.Value = _swap ? _values[1] : _values[0];
    }

    private void Init()
    {
        this.TileType = _opers[0];
        this.Value = _values[0];
        _swap = false;
    }

    #endregion
}