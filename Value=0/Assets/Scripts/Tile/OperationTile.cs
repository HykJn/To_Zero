using System;
using TMPro;
using UnityEngine;

public class OperationTile : MonoBehaviour
{
    #region ==========Properties==========

    public TileType TileType
    {
        get => oper;
        set
        {
            oper = value;
            portal.SetActive(value == TileType.Portal);
        }
    }

    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            text.text = oper switch
            {
                TileType.Add => $"+{value}",
                TileType.Sub => $"-{value}",
                TileType.Mul => $"×{value}",
                TileType.Div => $"÷{value}",
                TileType.Equal => $"={value}",
                TileType.Not => $"≠{value}",
                TileType.Greater => $">{value}",
                TileType.Less => $"<{value}",
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
            spriteRenderer.sprite = value ? onPlayer : @default;
            animator.enabled = !value;
            if (!value) animator.Play("Float", 0, UnityEngine.Random.Range(0, 1f));
        }
    }

    #endregion

    #region ==========Fields==========

    [SerializeField] protected TileType oper;
    [SerializeField] protected int value;
    [SerializeField] protected TMP_Text text;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Sprite @default, onPlayer;
    [SerializeField] protected Animator animator;
    [SerializeField] private GameObject portal;
    private bool _onPlayer;

    #endregion

    #region ==========Unity Events==========

    private void OnEnable()
    {
        GameManager.Instance.OnRestart += Init;
        Init();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRestart -= Init;
    }

    #endregion

    #region ==========Methods==========

    private void Init()
    {
        animator.Play("Float", 0, UnityEngine.Random.Range(0, 1f));
        OnPlayer = false;
    }

    #endregion
}