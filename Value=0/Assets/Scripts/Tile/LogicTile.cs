using System;
using TMPro;
using UnityEngine;

public class LogicTile : MonoBehaviour
{
    #region ==========Properties==========
    public Operator Operator { get => oper; set => oper = value; }
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            text.text = oper switch
            {
                Operator.Add => $"+{value}",
                Operator.Sub => $"-{value}",
                Operator.Mul => $"¡¿{value}",
                Operator.Div => $"¡À{value}",
                Operator.Portal => "P",
                Operator.Start => "S",
                _ => ""
            };
        }
    }
    #endregion

    #region ==========Fields==========
    [SerializeField] protected Operator oper;
    [SerializeField] protected int value;
    [SerializeField] protected TMP_Text text;
    #endregion
}
