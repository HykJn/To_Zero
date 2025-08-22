using TMPro;
using UnityEngine;
using static GlobalDefines;

public class Box : MonoBehaviour
{
    #region ==========Properties==========

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            spriteRenderer.sprite = value ? highlight : @default;
        }
    }

    #endregion

    #region ==========Fields==========

    [Header("Components"), SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text text_Value;
    [Header("References"), SerializeField] private GameObject[] previews;
    [SerializeField] private Sprite @default, highlight;

    private bool _isSelected;
    private Vector3 _startPos;

    #endregion

    #region ==========Unity Methods==========

    private void OnEnable()
    {
        GameManager.Instance.OnRestart += Restart;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRestart -= Restart;
    }

    #endregion

    #region ==========Methods==========

    public void Init(Vector3 startPos)
    {
        _startPos = startPos;
    }

    private void Restart()
    {
        this.transform.position = _startPos;
        Tile belowTile = GameManager.Instance.Stage.GetTile(this.transform.position);
        belowTile.Box = this;
        UpdateValue(belowTile);
    }

    private void UpdateValue(Tile belowTile)
    {
        text_Value.text = belowTile.GetValueToString();
    }

    public void OnHold(Vector3 playerPosition)
    {
        Vector3[] dirs = { Vector3.up, Vector3.left, Vector3.down, Vector3.right };
        for (int i = 0; i < 4; i++)
        {
            previews[i].SetActive(IsMovableDirection(dirs[i], playerPosition));
        }
    }

    public void Release()
    {
        foreach (GameObject obj in previews)
            obj.SetActive(false);
    }

    public bool Move(Vector3 direction, Vector3 playerPosition)
    {
        if (!IsMovableDirection(direction, playerPosition)) return false;
        GameManager.Instance.Stage.GetTile(this.transform.position).Box = null;
        this.transform.position += direction;
        Tile belowTile = GameManager.Instance.Stage.GetTile(this.transform.position);
        belowTile.Box = this;
        UpdateValue(belowTile);
        return true;
    }

    private bool IsMovableDirection(Vector3 direction, Vector3 playerPosition)
    {
        Vector3 pos = this.transform.position + direction;
        Stage stageInfo = GameManager.Instance.Stage;
        Tile tile = stageInfo.GetTile(pos);
        if (!tile) return false;
        return !tile.Box && tile.TileType != TileType.None && tile.TileType != TileType.Portal &&
               playerPosition != pos;
    }

    #endregion
}