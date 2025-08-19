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
        //player.GetComponent<Player>().Controllable = false;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        //player.GetComponent<Player>().Controllable = true;
    }

    #region ========== ButtonEvent ==========

    public void OnClick_Resume()
    {
        //SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        //inGameUIController.SetActive_PausePanel(false);

        UIManager.Instance.ClosePanel();
    }
    public void OnClick_Option()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        UIManager.Instance.Setting.SetActiveSettingPanel(true);
    }
    public void OnClick_Title()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);
        UIManager.Instance.ClosePanel();

        SceneManager.LoadScene("MainMenu");
        SoundManager.Instance.Play_BGM(BGMID.Title);
    }
    public void OnClick_Exit()
    {
        SoundManager.Instance.Play_UI_SFX(UISFXID.ButtonClick);

        Debug.Log("게임 종료 ");
        Application.Quit();
    }

    #endregion
}
