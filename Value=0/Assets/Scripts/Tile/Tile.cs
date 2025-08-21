using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using static GlobalDefines;

public class Tile : MonoBehaviour
{
    #region ==========Properties==========

    public TileType TileType => _tileTypes[_idx];

    public int Value => _values[_idx];

    public bool OnPlayer
    {
        get => _onPlayer;
        set
        {
            _onPlayer = value;
            spriteRenderer.sprite = value ? onPlayer : @default;
            AnimatorValidate();
        }
    }

    public Box Box
    {
        get => _box;
        set
        {
            _box = value;
            AnimatorValidate();
        }
    }

    #endregion

    #region ==========Fields==========

    [Header("Components"), SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text_Value;
    [Header("References"), SerializeField] private GameObject obj_Portal;
    [SerializeField] private Sprite @default, onPlayer;

    private int _idx;
    private TileType[] _tileTypes;
    private int[] _values;
    private bool _onPlayer;
    private Box _box;

    #endregion

    #region ==========Methods=========

    /// <summary>
    /// 초기화 함수, <see cref="Stage"/>에서 호출됨
    /// </summary>
    public void Init()
    {
        OnPlayer = TileType == TileType.Start;
    }

    /// <summary>
    /// 재시작 시 호출되는 함수, <see cref="Stage"/>에서 호출
    /// </summary>
    public void Restart()
    {
        Init();
        _box = null;
    }

    private void AnimatorValidate()
    {
        animator.enabled = !_onPlayer && !_box;
        if (animator.enabled)
            animator.Play(Animator.StringToHash("Float"), 0, Random.Range(0, 1f));
    }

    public void SetTile(string values)
    {
        string[] parts = values.Split(',');
        _tileTypes = new TileType[parts.Length];
        _values = new int[parts.Length];
        _idx = 0;

        for (int i = 0; i < parts.Length; i++)
        {
            obj_Portal.SetActive(false);
            switch (parts[i])
            {
                case "S":
                    _tileTypes[i] = TileType.Start;
                    break;
                case "P":
                    _tileTypes[i] = TileType.Portal;
                    obj_Portal.SetActive(true);
                    break;
                case "W":
                    _tileTypes[i] = TileType.None;
                    ObjectManager.Instance.GetObject(ObjectID.Wall, this.transform.position);
                    break;
                default:
                    _tileTypes[i] = parts[i][0] switch
                    {
                        '+' => TileType.Add,
                        '-' => TileType.Sub,
                        '*' => TileType.Mul,
                        '/' => TileType.Div,
                        '=' => TileType.Equal,
                        '!' => TileType.Not,
                        '>' => TileType.Greater,
                        '<' => TileType.Less,
                        _ => throw new InvalidOperationException("알 수 없는 기호입니다.")
                    };
                    _values[i] = int.Parse(parts[i][1..]);
                    break;
            }
        }

        UpdateText();
    }

    public string GetValueToString()
    {
        return text_Value.text;
    }

    /// <summary>
    /// 플레이어 이동 시 호출되는 함수, <see cref="Stage"/>에서 호출
    /// </summary>
    public void Swap()
    {
        if (_tileTypes.Length == 1) return;
        _idx = (_idx + 1) % _tileTypes.Length;
        UpdateText();
    }

    private void UpdateText()
    {
        text_Value.text = _tileTypes[_idx] switch
        {
            TileType.None => string.Empty,
            TileType.Start => string.Empty,
            TileType.Portal => string.Empty,
            TileType.Add => "+" + _values[_idx],
            TileType.Sub => "-" + _values[_idx],
            TileType.Mul => "×" + _values[_idx],
            TileType.Div => "÷" + _values[_idx],
            TileType.Equal => "=" + _values[_idx],
            TileType.Not => "≠" + _values[_idx],
            TileType.Greater => ">" + _values[_idx],
            TileType.Less => "<" + _values[_idx],
            _ => throw new InvalidOperationException("알 수 없는 기호입니다.")
        };
    }

    #endregion
}