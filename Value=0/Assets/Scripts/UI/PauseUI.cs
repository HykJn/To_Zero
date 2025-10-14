using UnityEngine;
using static GLOBAL;

public class PauseUI : MonoBehaviour, IPanel
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public void Open()
    {
        UIManager.Instance.OpenPanel(this);
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        UIManager.Instance.ClosePanel(this);
        this.gameObject.SetActive(false);
    }

    public void ForceClose()
    {
        this.gameObject.SetActive(false);
    }

    public void OnClick_Resume()
    {
        Close();
    }

    public void OnClick_Options()
    {
        UIManager.Instance.OptionPanel.Open();
    }

    public void OnClick_Title()
    {
        StartCoroutine(UIManager.Instance.LoadScene(SceneID.Title));
        Close();
    }

    public void OnClick_Exit()
    {
        //TODO: Saving before exit
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    #endregion
}