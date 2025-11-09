using System;
using UnityEngine;

public class EndingPanel : MonoBehaviour, IPanel
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [SerializeField] private GameObject askPanel;

    #endregion

    #region =====Unity Events=====

    private void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if (Input.anyKeyDown)
        {
            askPanel.SetActive(true);
        }
    }

    #endregion

    #region =====Methods=====

    public void Open()
    {
        UIManager.Instance.OpenPanel(this);
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
    }

    public void ForceClose()
    {
    }

    public void OnClick_Accept()
    {
        SequanceManager.Chapter = 0;
        SequanceManager.Stage = 1;
        SequanceManager.LastDialog = 0;
        SaveManager.Save();
        UIManager.Instance.LoadScene(GLOBAL.SceneID.Title);
    }

    public void OnClick_Deny()
    {
        SaveManager.Save();
        UIManager.Instance.LoadScene(GLOBAL.SceneID.Title);
    }

    #endregion
}