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
            spriteRenderer.sprite = value ? sprite_Light : sprite_Default;

            if (Operator == Operation.Portal) return;
            if (!value) animator.Play(Animator.StringToHash("Float"), 0, Random.Range(0, 1f));
        }
    }

    public int WarningCount
    {
        get => _warningCount;
        set
        {
            _warningCount = Mathf.Clamp(value, 0, int.MaxValue);
            if (Operator == Operation.Portal) return;
            if (_warningCount > 0) spriteRenderer.sprite = sprite_Warning;
            else spriteRenderer.sprite = AnyObjectAbove ? sprite_Light : sprite_Default;
        }
    }

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] protected TMP_Text text_Value;
    [SerializeField] private GameObject portal;

    [Header("Sprites")]
    [SerializeField] private Sprite sprite_Default;
    [SerializeField] private Sprite sprite_Warning;
    [SerializeField] private Sprite sprite_Light;

    private bool _anyObjectAbove;
    private int _warningCount;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public override void Init(string value)
    {
        OnRestart();
        spriteRenderer.sprite = sprite_Default;

        Value = 0;
        text_Value.text = string.Empty;
        portal.SetActive(value.Equals("P"));
        if (value is "P")
        {
            Operator = Operation.Portal;
            return;
        }

        if (value is "N" or "S")
        {
            Operator = Operation.None;
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