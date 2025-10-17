using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour, IPanel
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text desc;
    [SerializeField] private Button next, prev;
    [SerializeField] private TutorialGroup[] tutorials;

    private TutorialGroup _currentTutorial;
    private int _curIdx;

    #endregion

    #region =====Unity Events=====

    private void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if (!_currentTutorial) return;

        if (_currentTutorial.Length > 1)
        {
            if (_curIdx == 0)
            {
                next.gameObject.SetActive(true);
                prev.gameObject.SetActive(false);
            }
            else if (_curIdx == _currentTutorial.Length - 1)
            {
                next.gameObject.SetActive(false);
                prev.gameObject.SetActive(true);
            }
            else
            {
                next.gameObject.SetActive(true);
                prev.gameObject.SetActive(true);
            }
        }
        else
        {
            next.gameObject.SetActive(false);
            prev.gameObject.SetActive(false);
        }

        TutorialInfo info = _currentTutorial.Tutorials[_curIdx];
        image.sprite = info.Image;
        title.text = info.Name;
        desc.text = info.Description;
    }

    #endregion

    #region =====Methods=====

    public void Open()
    {
        UIManager.Instance.OpenPanel(this);
        this.gameObject.SetActive(true);
        SoundManager.Instance.Play(GLOBAL.UI_SFX_ID.PanelOpen);
    }

    public void Close()
    {
        UIManager.Instance.ClosePanel(this);
        this.gameObject.SetActive(false);
        SoundManager.Instance.Play(GLOBAL.UI_SFX_ID.PanelClose);
    }

    public void ForceClose()
    {
        this.gameObject.SetActive(false);
        SoundManager.Instance.Play(GLOBAL.UI_SFX_ID.PanelClose);
    }

    public void OnClick_Next()
    {
        if (!_currentTutorial) return;
        _curIdx = Mathf.Clamp(_curIdx + 1, 0, _currentTutorial.Length - 1);
    }

    public void OnClick_Prev()
    {
        if (!_currentTutorial) return;
        _curIdx = Mathf.Clamp(_curIdx - 1, 0, _currentTutorial.Length - 1);
    }

    public void SetTutorial(int index)
    {
        _currentTutorial = tutorials[index];
        _curIdx = 0;

        Open();
    }

    #endregion
}