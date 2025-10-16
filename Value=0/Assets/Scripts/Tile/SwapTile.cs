using System;
using System.Linq;
using TMPro;
using UnityEngine;
using static GLOBAL;

public class SwapTile : OperationTile
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [SerializeField] private TMP_Text text_Next;

    private Operation[] _operators;
    private int[] _values;
    private int _idx;

    #endregion

    #region =====Unity Events=====

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.Player.OnPlayerMove += Swap;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.Player.OnPlayerMove -= Swap;
    }

    #endregion

    #region =====Methods=====

    public override void Init(string value)
    {
        string[] values = value.Split(',');
        _operators = values.Select(s => s[0] switch
        {
            '+' => Operation.Add,
            '-' => Operation.Subtract,
            '*' => Operation.Multiply,
            '/' => Operation.Divide,
            '=' => Operation.Equal,
            '!' => Operation.NotEqual,
            '>' => Operation.Greater,
            '<' => Operation.Less,
            _ => Operation.None
        }).ToArray();

        _values = values.Select(s => int.Parse(s[1..])).ToArray();

        Swap(0);
    }

    protected override void OnRestart()
    {
        base.OnRestart();
        Swap(0);
    }

    public void Swap() => Swap((_idx + 1) % 2);

    public void Swap(int idx)
    {
        if (idx is < 0 or > 1) throw new IndexOutOfRangeException();
        _idx = idx;
        Operator = _operators[idx];
        Value = _values[idx];

        text_Value.text = Operator switch
        {
            Operation.Add => "+",
            Operation.Subtract => "-",
            Operation.Multiply => "×",
            Operation.Divide => "÷",
            Operation.Equal => "=",
            Operation.NotEqual => "≠",
            Operation.Greater => ">",
            Operation.Less => "<",
            _ => null
        } + Value;

        text_Next.text = _operators[(_idx + 1) % 2] switch
        {
            Operation.Add => "+",
            Operation.Subtract => "-",
            Operation.Multiply => "×",
            Operation.Divide => "÷",
            Operation.Equal => "=",
            Operation.NotEqual => "≠",
            Operation.Greater => ">",
            Operation.Less => "<",
            _ => null
        } + _values[(_idx + 1) % 2];
    }

    #endregion
}