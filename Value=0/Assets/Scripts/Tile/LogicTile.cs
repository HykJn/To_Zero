using UnityEngine;

public class LogicTile : MonoBehaviour
{
    #region ==========Properties==========
    public Operator Operator => oper;
    public int Value => value;
    #endregion

    #region ==========Fields==========
    [SerializeField] private Operator oper;
    [SerializeField] private int value;
    #endregion
}
