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
        UIManager.Instance.LoadScene(SceneID.Office, afterLoad: AfterLoad);
        return;

        void AfterLoad()
        {
            if (SequanceManager.Chapter == 0 && SequanceManager.Stage == 1)
            {
                UIManager.Instance.DialogPanel.SetDialog(0);
                UIManager.Instance.DialogPanel.StartDialog();
            }
        }
    }

    //������
    public void OnClick_Boss()
    {
        UIManager.Instance.LoadScene(SceneID.Boss);
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

    public void OnClick_Remove()
    {
        SequanceManager.Chapter = 0;
        SequanceManager.Stage = 1;
    }

    #endregion
}