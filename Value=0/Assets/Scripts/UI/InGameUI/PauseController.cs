using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private InGameUIController inGameUIController;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        player.GetComponent<Player>().Controllable = false;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        player.GetComponent<Player>().Controllable = true;
    }

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
