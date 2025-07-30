using System;
using TMPro;
using UnityEngine;

public class OperationTile : MonoBehaviour
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
                Operator.Equal => $"={value}",
                Operator.Not => $"¡Á{value}",
                Operator.Greater => $">{value}",
                Operator.Less => $"<{value}",
                Operator.Portal => "P",
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
