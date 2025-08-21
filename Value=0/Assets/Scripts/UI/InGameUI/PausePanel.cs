using UnityEngine;
using static GlobalDefines;

public class PausePanel : Panel
{
    [SerializeField] private InGameUIController inGameUIController;

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    #region ========== ButtonEvent ==========

    public void OnClick_Resume()
    {
        ClosePanel();
    }

    public void OnClick_Option()
    {
        UIManager.Instance.SettingPanel.OpenPanel();

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void OnClick_Title()
    {
        UIManager.Instance.LoadScene(SceneID.Title);

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void OnClick_Exit()
    {
        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    #endregion
}