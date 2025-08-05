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
            this.GetComponentInChildren<Renderer>().material = value ? selectedMaterial : defaultMaterial;
        }
    }
    #endregion

    #region ==========Fields==========
    [SerializeField] private Material defaultMaterial, selectedMaterial;
    private bool isSelected = false;
    #endregion

    #region ==========Unity Methods==========

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
    #endregion
}
