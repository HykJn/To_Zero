using TMPro;
using UnityEngine;

public class LogicTile : MonoBehaviour
{
    #region ==========Properties==========
    public Operator Operator { get => oper; set => oper = value; }
    public int Value { get => value; set => this.value = value; }
    public TMP_Text Text => text;
    #endregion

    #region ==========Fields==========
    [SerializeField] private Operator oper;
    [SerializeField] private int value;
    [SerializeField] private TMP_Text text;
    #endregion
}
