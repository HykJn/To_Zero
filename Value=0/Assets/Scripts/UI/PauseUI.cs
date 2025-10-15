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
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        Close();
    }

    public void OnClick_Options()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        UIManager.Instance.OptionPanel.Open();
    }

    public void OnClick_Title()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        UIManager.Instance.LoadScene(SceneID.Title);
        Close();
    }

    public void OnClick_Exit()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        //TODO: Saving before exit
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    #endregion
}