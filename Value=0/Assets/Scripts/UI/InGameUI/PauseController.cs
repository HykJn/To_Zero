using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private InGameUIController inGameUIController;

    #region ========== ButtonEvent ==========

    public void OnClick_Resume()
    {
        inGameUIController.SetActive_PausePanel(false);
    }
    public void OnClick_Option()
    {
        SettingManager.instance.SetActiveSettingPanel(true);
    }
    public void OnClick_Title()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void OnClick_Exit()
    {
        Debug.Log("게임 종료 ");
        Application.Quit();
    }
        
    #endregion
}
