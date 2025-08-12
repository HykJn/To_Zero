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
                Operator.Mul => $"×{value}",
                Operator.Div => $"÷{value}",
                Operator.Equal => $"={value}",
                Operator.Not => $"≠{value}",
                Operator.Greater => $">{value}",
                Operator.Less => $"<{value}",
                Operator.Portal => "P",
                _ => ""
            };
        }
    }
    public bool OnPlayer
    {
        get => _onPlayer;
        set
        {
            _onPlayer = value;
            this.GetComponentInChildren<SpriteRenderer>().sprite = value ? onPlayer : _default;
            this.GetComponent<Animator>().enabled = !value;
            if (!value) this.GetComponent<Animator>().Play("Float", 0, UnityEngine.Random.Range(0, 1f));
        }
    }
    #endregion

    #region ==========Fields==========
    [SerializeField] protected Operator oper;
    [SerializeField] protected int value;
    [SerializeField] protected TMP_Text text;
    [SerializeField] protected Sprite _default, onPlayer;
    private bool _onPlayer;
    #endregion

    private void OnEnable()
    {
        this.GetComponent<Animator>().Play("Float", 0, UnityEngine.Random.Range(0, 1f));
        OnPlayer = false;
    }
}
