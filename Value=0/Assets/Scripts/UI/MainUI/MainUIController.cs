using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject StagePanel;

    [Header("Stage")]
    [Tooltip("제어할 Scroll Rect를 여기에 연결하세요.")]
    [SerializeField]
    private ScrollRect targetScrollRect;

    [Tooltip("버튼을 한 번 클릭했을 때 스크롤될 양 (0~1 사이 값)")]
    [SerializeField]
    private float scrollStep = 0.1f;

    void Start()
    {
        SettingManager.instance.SetActiveSettingPanel(false);

        StagePanel.SetActive(false);

        MenuPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!StagePanel.activeSelf) return;
            OnClick_StagePanelExit();
        }
    }


    #region  =====ButtonEvent=====

    public void OnClick_Start()
    {
        //MenuPanel.SetActive(false);

        //StagePanel.SetActive(true);

        LoadingPanel.instance.Loading(1);
    }
    public void OnClick_StagePanelExit()
    {
        StagePanel.SetActive(false);

        MenuPanel.SetActive(true);
    }
    public void OnClick_CreativeMode()
    {
        Debug.Log("창작마당");
    }
    public void OnClick_Setting()
    {
        SettingManager.instance.SetActiveSettingPanel(true);
    }
    public void Onclick_Quit()
    {
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


    public void OnClick_Stage(int stageNum)
    {
        Debug.Log($"Stage{stageNum}로 이동");
        LoadingPanel.instance.Loading(stageNum);
    }
    #endregion
}
