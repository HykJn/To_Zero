using TMPro;
using UnityEngine;

public class Box : MonoBehaviour
{
    #region ==========Properties==========
    public bool Selected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            this.GetComponentInChildren<SpriteRenderer>().sprite = isSelected ? outline : _default;
        }
    }
    #endregion

    #region ==========Fields==========
    [SerializeField] private Sprite _default, outline;
    [SerializeField] private Transform preview;
    private bool isSelected = false;
    #endregion

    #region ==========Unity Methods==========
    private void OnEnable()
    {
        Selected = false;
    }

    private void OnDisable()
    {
        Selected = false;
    }
    #endregion

    #region ==========Methods==========
    public void UpdateValue()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position + Vector3.back, Vector3.forward, 5f, LayerMask.GetMask("Tile"));
        if (hit)
        {
            GameObject obj = hit.transform.parent.gameObject; ;

            this.GetComponentInChildren<TMP_Text>().text =
                obj.GetComponentInChildren<TMP_Text>().text;
        }
    }

    public void SetPreview(Vector3 position)
    {
        int idx = -1;
        if (position == Vector3.up) idx = 0;
        else if (position == Vector3.left) idx = 1;
        else if (position == Vector3.down) idx = 2;
        else if (position == Vector3.right) idx = 3;

        if (idx == -1) return;

        preview.GetChild(idx).gameObject.SetActive(true);
    }

    public void ClearPreview()
    {
        foreach (Transform child in preview)
        {
            child.gameObject.SetActive(false);
        }
    }
    #endregion
}
