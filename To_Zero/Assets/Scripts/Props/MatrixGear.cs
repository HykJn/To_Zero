using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GLOBAL;

public class MatrixGear : MonoBehaviour, IInteractable
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text_Space;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public void Notify(bool flag)
    {
        if (SequanceManager.LastDialog is 13 or 14 or 22 or 23 or 32 or 33 or 45) return;
        text_Space.enabled = flag;
        animator.SetTrigger(Animator.StringToHash(flag ? "Open" : "Close"));
    }

    public void Interact()
    {
        if (SequanceManager.LastDialog is 14 or 23 or 33) return;
        UIManager.Instance.LoadScene(SceneID.Matrix, afterLoad: AfterLoad);
        return;

        void AfterLoad()
        {
            GameManager.Instance.StageNumber = SequanceManager.Stage;
            GameManager.Instance.LoadDialog();
        }
    }

    #endregion
}