using UnityEngine;
using static GLOBAL;

public class TitleUI : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public void OnClick_Start()
    {
        SoundManager.Instance.Play(UI_SFX_ID.StartButtonClick);
        SoundManager.Instance.Stop(AudioChannel.BGM);
        UIManager.Instance.LoadScene(SceneID.Office, afterLoad: () => SoundManager.Instance.Play(BGM_ID.Matrix));
        return;
    }

    public void OnClick_CreativeMode()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        print("Creative Mode");
    }

    public void OnClick_Options()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
        UIManager.Instance.OptionPanel.Open();
    }

    public void OnClick_Quit()
    {
        SoundManager.Instance.Play(UI_SFX_ID.ButtonClick);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    #endregion
}