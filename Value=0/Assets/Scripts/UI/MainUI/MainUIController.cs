using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static GlobalDefines;

public class MainUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private Panel menuPanel;
    [SerializeField] private Panel stagePanel;

    [Header("Stage")]
    [Tooltip("제어할 Scroll Rect를 여기에 연결하세요.")]
    [SerializeField] private ScrollRect targetScrollRect;

    [Tooltip("버튼을 한 번 클릭했을 때 스크롤될 양 (0~1 사이 값)")]
    [SerializeField] private float scrollStep = 0.1f;

    private void Start()
    {
        UIManager.Instance.MainUI = this;

        stagePanel.gameObject.SetActive(false);

        menuPanel.gameObject.SetActive(true);
    }

    #region =====ButtonEvent=====

    public void OnClick_Start()
    {
        OnClick_Stage(1);
    }

    public void OnClick_Stage(int stageNum)
    {
        Debug.Log($"Stage{stageNum}로 이동");

        UIManager.Instance.ToPlay(stageNum);

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.StartButtonClick);
    }

    public void OnClick_StagePanelExit()
    {
        stagePanel.gameObject.SetActive(false);

        menuPanel.gameObject.SetActive(true);

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void OnClick_CreativeMode()
    {
        Debug.Log("창작마당");

        //TODO: Implement custom map

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void OnClick_Setting()
    {
        UIManager.Instance.SettingPanel.OpenPanel();

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);
    }

    public void Onclick_Quit()
    {
        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);

        Debug.Log("게임 종료");
        //Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void ScrollRight()
    {
        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);

        if (targetScrollRect == null)
        {
            Debug.LogError("제어할 Scroll Rect가 지정되지 않았습니다!");
            return;
        }

        // 현재 스크롤 위치(0.0 ~ 1.0)에 정해진 step 값을 더합니다.
        float newPosition = targetScrollRect.horizontalNormalizedPosition + scrollStep;
        // 계산된 위치가 0과 1 사이를 벗어나지 않도록 Clamp01 함수로 고정합니다.
        targetScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPosition);
    }

    public void ScrollLeft()
    {
        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.ButtonClick);

        if (targetScrollRect == null)
        {
            Debug.LogError("제어할 Scroll Rect가 지정되지 않았습니다!");
            return;
        }

        // 현재 스크롤 위치(0.0 ~ 1.0)에서 정해진 step 값을 뺍니다.
        float newPosition = targetScrollRect.horizontalNormalizedPosition - scrollStep;
        // 계산된 위치가 0과 1 사이를 벗어나지 않도록 Clamp01 함수로 고정합니다.
        targetScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPosition);
    }

    #endregion
}