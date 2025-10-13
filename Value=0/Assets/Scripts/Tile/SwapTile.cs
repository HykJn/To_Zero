using System.Linq;
using UnityEngine;
using static GLOBAL;

public class SwapTile : OperationTile
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    private Operation[] _operators;
    private int[] _values;
    private bool _swap;

    #endregion

    #region =====Unity Events=====

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

        Swap(false);
    }

    protected override void OnRestart()
    {
        Swap(false);
    }

    public void Swap() => Swap(!_swap);

    public void Swap(bool swap)
    {
        _swap = swap;
        Operator = _swap ? _operators[0] : _operators[1];
        Value = _swap ? _values[0] : _values[1];

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
    }

    #endregion
}