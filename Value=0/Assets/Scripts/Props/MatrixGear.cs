using System;
using TMPro;
using UnityEngine;
using static GLOBAL;

public class MatrixGear : MonoBehaviour, IInteractable
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [SerializeField] private TMP_Text text_Space;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public void Notify(bool flag)
    {
        text_Space.enabled = flag;
    }

    public void Interact()
    {
        UIManager.Instance.LoadScene(SceneID.Matrix);
    }

    #endregion
}