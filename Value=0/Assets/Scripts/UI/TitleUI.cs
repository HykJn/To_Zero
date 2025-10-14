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
        UIManager.Instance.LoadScene(SceneID.Office);
    }

    public void OnClick_CreativeMode()
    {
        print("Creative Mode");
    }

    public void OnClick_Options()
    {
        UIManager.Instance.OptionPanel.Open();
    }

    public void OnClick_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    #endregion
}