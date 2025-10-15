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

    //º¸½ºÀü
    public void OnClick_Boss()
    {
        UIManager.Instance.LoadScene(SceneID.Boss);
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