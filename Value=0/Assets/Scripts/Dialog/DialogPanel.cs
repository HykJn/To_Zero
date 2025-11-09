using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GLOBAL;


public class DialogPanel : MonoBehaviour, IPanel
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image image_Left;
    [SerializeField] private Image image_Right;
    [SerializeField] private TMP_Text text_Name;
    [SerializeField] private TMP_Text text_Dialog;

    private Dictionary<int, DialogGroup> _dialogs;
    private Dictionary<string, CharacterInfo> _characterInfos;
    private DialogGroup _currentDialog;
    private int _currentDialogIdx;
    private bool _onTyping;

    private readonly Color _disabled = new(100 / 255f, 100 / 255f, 100 / 255f, 255 / 255f);

    #endregion

    #region =====Unity Events=====

    private void Awake()
    {
        _dialogs = Resources.LoadAll<DialogGroup>("Dialogs").ToDictionary(x => x.DialogID);
        _characterInfos = Resources.LoadAll<CharacterInfo>("Characters").ToDictionary(x => x.name);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;
        if (UIManager.Instance.PausePanel.gameObject.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_onTyping) StopTyping();
            else NextDialog();
        }
    }

    #endregion

    #region =====Methods=====

    public void Open()
    {
        UIManager.Instance.OpenPanel(this);
        panel.SetActive(true);
    }

    public void Close()
    {
        UIManager.Instance.ClosePanel(this);
        panel.SetActive(false);
    }

    public void ForceClose()
    {
        panel.SetActive(false);
    }

    public void SetDialog(int dialogID)
    {
        if (!_dialogs.TryGetValue(dialogID, out _currentDialog)) return;

        //Init fields
        _currentDialogIdx = 0;
        image_Left.color = _disabled;
        image_Right.color = _disabled;
        text_Name.text = string.Empty;
        text_Dialog.text = string.Empty;
    }

    public void StartDialog()
    {
        if (!panel.activeSelf) Open();
        NextDialog();
    }

    public void StopDialog()
    {
        StopAllCoroutines();
        UIManager.Instance.ClosePanel(this);
        if (!_currentDialog) return;
        if (_currentDialogIdx < _currentDialog.Dialogs.Length) return;
        SequanceManager.LastDialog = _currentDialog.DialogID;
        print("Save Dialog " + SequanceManager.LastDialog);
        if (SequanceManager.LastDialog == 10)
        {
            UIManager.Instance.MatrixUI.TutorialPanel.SetTutorial(0);
        }
        else if (SequanceManager.LastDialog == 11)
        {
            UIManager.Instance.MatrixUI.TutorialPanel.SetTutorial(1);
        }
        else if (SequanceManager.LastDialog == 12)
        {
            UIManager.Instance.MatrixUI.TutorialPanel.SetTutorial(2);
        }
        else if (SequanceManager.LastDialog == 21)
        {
            UIManager.Instance.MatrixUI.TutorialPanel.SetTutorial(3);
        }
        else if (SequanceManager.LastDialog == 31)
        {
            UIManager.Instance.MatrixUI.TutorialPanel.SetTutorial(4);
        }
        else if (SequanceManager.LastDialog == 13)
        {
            SequanceManager.Chapter = 2;
            UIManager.Instance.LoadScene(SceneID.Office);
        }
        else if (SequanceManager.LastDialog == 14)
        {
            SequanceManager.Stage = 7;
        }
        else if (SequanceManager.LastDialog == 22)
        {
            SequanceManager.Chapter = 3;
            UIManager.Instance.LoadScene(SceneID.Office);
        }
        else if (SequanceManager.LastDialog == 23)
        {
            SequanceManager.Stage = 13;
        }
        else if (SequanceManager.LastDialog == 32)
        {
            SequanceManager.Chapter = 4;
            UIManager.Instance.LoadScene(SceneID.Office);
        }
        else if (SequanceManager.LastDialog == 33)
        {
            SequanceManager.Stage = 17;
        }
        else if (SequanceManager.LastDialog == 45)
        {
            SequanceManager.Chapter = 5;
            UIManager.Instance.LoadScene(SceneID.Office);
        }
    }

    private void NextDialog()
    {
        if (_currentDialogIdx >= _currentDialog.Dialogs.Length)
        {
            StopDialog();
            return;
        }

        //Init
        Dialog dialog = _currentDialog.Dialogs[_currentDialogIdx];
        text_Dialog.text = string.Empty;
        image_Left.enabled = true;
        image_Right.enabled = true;
        
        //Set Color
        image_Left.color = dialog.OnLeftSide ? Color.white : _disabled;
        image_Right.color = dialog.OnLeftSide ? _disabled : Color.white;
        
        //Set Sprite
        Image curSide = dialog.OnLeftSide ? image_Left : image_Right;
        curSide.sprite = _characterInfos[dialog.Character.ToString()][dialog.Face];
        
        switch (dialog.Character)
        {
            case Character.Zero:
                text_Name.text = "제로";
                break;
            case Character.Pix:
                text_Name.text = "픽스";
                break;
            case Character.Panos:
                text_Name.text = "파노스";
                break;
            case Character.Anchor:
                image_Left.enabled = false;
                image_Right.enabled = false;
                text_Name.text = "뉴스 앵커";
                break;
            case Character.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        StartCoroutine(Typing(dialog.Text));
        _currentDialogIdx++;
    }

    private void StopTyping()
    {
        _onTyping = false;
        StopAllCoroutines();
        text_Dialog.text = _currentDialog.Dialogs[_currentDialogIdx - 1].Text;
    }

    private IEnumerator Typing(string text)
    {
        _onTyping = true;
        foreach (char c in text)
        {
            text_Dialog.text += c;
            SoundManager.Instance.PlayOneShot(UI_SFX_ID.DialogTyping);
            yield return new WaitForSeconds(0.1f);
        }

        text_Dialog.text = text;
        _onTyping = false;
    }

    #endregion
}