using System;
using UnityEngine;

public class SwapTile : OperationTile
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========
    private bool swap;
    private int[] values = new int[2];
    private Operator[] opers = new Operator[2];
    #endregion

    #region ==========Unity Methods==========

    #endregion

    #region ==========Methods==========
    public void SetTile(string values)
    {
        string[] parts = values.Split(",");
        for(int i = 0; i < 2; i++)
        {
            opers[i] = parts[i][0] switch
            {
                '+' => Operator.Add,
                '-' => Operator.Sub,
                '*' => Operator.Mul,
                '/' => Operator.Div,
                _ => throw new InvalidOperationException("Invalid operator in swap tile.")
            };
            this.values[i] = int.Parse(parts[i][1..]);
        }
        this.Operator = opers[0];
        this.Value = this.values[0];
    }

    public void Swap()
    {
        this.Operator = swap ? opers[0] : opers[1];
        this.Value = swap ? values[0] : values[1];
        swap = !swap;
    }
    #endregion
}
