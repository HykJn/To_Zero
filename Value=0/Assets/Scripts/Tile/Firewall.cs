using System;
using UnityEngine;

public class Firewall : MonoBehaviour
{
    #region =====Properties=====

    public Vector2 Position { get; private set; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            spriteRenderer.sprite = value ? highlightedSprite : defaultSprite;
        }
    }

    public bool IsHeld { get; private set; }

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private GameObject[] previews; //0: Up, 1: Left, 2: Down, 3: Right

    private bool _isSelected;
    private Vector2 _startPos;

    #endregion

    #region =====Unity Events=====

    private void OnEnable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.OnRestart += OnRestart;
    }

    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.OnRestart -= OnRestart;
    }

    #endregion

    #region =====Methods=====

    public void Init(Vector2 pos)
    {
        Position = pos;
        _startPos = pos;
        IsSelected = false;
        IsHeld = false;
        OnRelease();
        GameManager.Instance.Stage.GetTile<OperationTile>(Position).AnyObjectAbove = true;
    }

    public void Move(Vector2 pos)
    {
        if (!CheckIsMovable(pos)) return;

        OperationTile below = GameManager.Instance.Stage.GetTile<OperationTile>(Position) as OperationTile;
        below!.AnyObjectAbove = false;

        this.transform.position = pos;
        Position = pos;
        IsSelected = false;
        OnRelease();

        below = GameManager.Instance.Stage.GetTile<OperationTile>(Position) as OperationTile;
        below!.AnyObjectAbove = true;
    }

    private bool CheckIsMovable(Vector2 pos)
    {
        Stage stage = GameManager.Instance.Stage;
        return !stage.TryGetFirewall(pos, out Firewall firewall) && stage.TryGetTile<Tile>(pos, out _);
    }

    public void OnHeld()
    {
        IsHeld = true;
        Vector2[] pos = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
        for (int i = 0; i < 4; i++)
            previews[i].SetActive(CheckIsMovable((Vector2)this.transform.position + pos[i]));
        this.transform.localScale = new(1.1f, 1.1f, 1.1f);
    }

    public void OnRelease()
    {
        IsHeld = false;
        foreach (GameObject preview in previews)
            preview.SetActive(false);
        this.transform.localScale = Vector3.one;
    }

    private void OnRestart()
    {
        IsSelected = false;
        OnRelease();

        this.transform.position = _startPos;
        Position = _startPos;

        GameManager.Instance.Stage.GetTile<OperationTile>(Position).AnyObjectAbove = true;
    }

    #endregion
}