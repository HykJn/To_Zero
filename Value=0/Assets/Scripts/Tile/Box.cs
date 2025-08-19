using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour
{
    #region ==========Properties==========

    public bool Selected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            this.transform.localScale = value ? new Vector3(1.1f, 1.1f, 1) : Vector3.one;
            spriteRenderer.sprite = _isSelected ? outline : @default;
            SetPreview(value);
        }
    }

    #endregion

    #region ==========Fields==========

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite @default, outline;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject[] previews;

    private Vector3 _initPos;
    private bool _isSelected = false;

    #endregion

    #region ==========Unity Methods==========

    private void OnEnable()
    {
        GameManager.Instance.OnRestart += Init;
        Selected = false;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRestart -= Init;
        Selected = false;
    }

    #endregion

    #region ==========Methods==========

    public void Init(Vector3 initPos)
    {
        _initPos = initPos;
        _isSelected = false;
        UpdateValue();
    }

    private void Init()
    {
        this.transform.position = _initPos;
        _isSelected = false;
        UpdateValue();
    }

    public void UpdateValue()
    {
        OperationTile tile = GameManager.Instance.CurrentStage.GetTile(this.transform.position);
        text.text = tile ? tile.GetComponentInChildren<TMP_Text>().text : string.Empty;
    }

    public void SetPreview(bool active)
    {
        if (!active)
        {
            foreach (GameObject preview in previews)
                preview.SetActive(false);
        }
        else
        {
            Vector3[] directions = { Vector3.up, Vector3.left, Vector3.down, Vector3.right };
            for (int i = 0; i < 4; i++)
            {
                previews[i].SetActive(
                    GameManager.Instance.CurrentStage.IsMovable(this.transform.position + directions[i]) &&
                    GameManager.Instance.CurrentStage.GetTile(this.transform.position + directions[i]).TileType !=
                    TileType.Portal
                );
            }
        }
    }

    public void Move(Vector3 direction)
    {
        if (!Selected) return;
        if (!GameManager.Instance.CurrentStage.IsMovable(this.transform.position + direction) ||
            GameManager.Instance.CurrentStage.GetTile(this.transform.position + direction).TileType ==
            TileType.Portal) return;

        GameManager.Instance.CurrentStage.GetTile(this.transform.position).GetComponent<Animator>().enabled = false;
        this.transform.position += direction;
        GameManager.Instance.CurrentStage.GetTile(this.transform.position).GetComponent<Animator>().enabled = true;
        UpdateValue();
        Selected = false;
    }

    #endregion
}