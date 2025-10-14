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
    public string Text => text_Value.text;

    public bool AnyObjectAbove
    {
        get => _anyObjectAbove;
        set
        {
            _anyObjectAbove = value;
            text_Value.enabled = !value;
            animator.enabled = !value;
            if (animator.enabled)
            {
                animator.Play(Animator.StringToHash("Float"), 0, Random.Range(0, 1f));
            }
        }
    }

    public int WarningCount
    {
        get => _warningCount;
        set
        {
            _warningCount = Mathf.Clamp(value, 0, int.MaxValue);
            if (value > 0) spriteRenderer.sprite = sprite_Warning;
            else spriteRenderer.sprite = Operator == Operation.Portal ? sprite_Portal : sprite_Default;
        }
    }

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] protected TMP_Text text_Value;

    [Header("Sprites")]
    [SerializeField] private Sprite sprite_Default;
    [SerializeField] private Sprite sprite_Warning;
    [SerializeField] private Sprite sprite_Portal;

    private bool _anyObjectAbove;
    private int _warningCount;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public override void Init(string value)
    {
        OnRestart();

        if (value is "P" or "N" or "S")
        {
            Operator = value is "P" ? Operation.Portal : Operation.None;
            spriteRenderer.sprite = value is "P" ? sprite_Portal : sprite_Default;
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
        WarningCount = 0;
        AnyObjectAbove = false;
    }

    #endregion
}