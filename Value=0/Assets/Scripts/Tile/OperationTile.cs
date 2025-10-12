using System;
using TMPro;
using UnityEngine;
using static GLOBAL;
using Random = UnityEngine.Random;

public class OperationTile : Tile
{
    #region =====Properties=====

    public Operation Operator { get; protected set; }
    public int Value { get; protected set; }

    public bool AnyObjectAbove
    {
        get => _anyObjectAbove;
        set
        {
            _anyObjectAbove = value;
            text_Value.enabled = !value;
        }
    }

    public bool Warning
    {
        get => _warning;
        set
        {
            _warning = value;
            spriteRenderer.sprite = value ? sprite_Warning : sprite_Default;
        }
    }

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] protected TMP_Text text_Value;

    [Header("Sprites")]
    [SerializeField] private Sprite sprite_Default;
    [SerializeField] private Sprite sprite_Warning;

    private bool _anyObjectAbove = false;
    private bool _warning = false;

    #endregion

    #region =====Unity Events=====

    protected override void OnEnable()
    {
        return;
    }

    protected override void OnDisable()
    {
        this.enabled = false;
    }

    #endregion

    #region =====Methods=====

    public override void Init(string value)
    {
        spriteRenderer.sprite = sprite_Default;

        if (value is "P" or "N" or "S")
        {
            Operator = value is "P" ? Operation.Portal : Operation.None;
            Value = 0;
            text_Value.text = string.Empty;
            return;
        }


        Operator = value[0] switch
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
        };

        Value = int.Parse(value[1..]);

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

    protected override void OnRestart()
    {
        //Nothing
    }

    #endregion
}