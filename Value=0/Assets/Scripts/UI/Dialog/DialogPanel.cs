using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalDefines;

[System.Serializable]
public struct DialogueData
{
    public Character character;
    [TextArea(3, 5)] public string dialogue; // 대사
}

public class DialogPanel : Panel
{
    #region ==========Fields==========

    [Header("References")]
    [SerializeField] private Image player_LD, system_LD;

    [SerializeField] private Image imageDialog; // 공통 대화창 이미지
    [SerializeField] private TextMeshProUGUI textName; // 공통 캐릭터 이름
    [SerializeField] private TextMeshProUGUI textDialogue; // 공통 대사 출력 text

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.1f;
    private int _dialogIdx; // 현재 대사순번

    private DialogueData[] _dialogs; // 현재 분기의 대사 목록
    private bool _onTyping;

    #endregion

    #region ==========Unity Methods==========

    private void Update()
    {
        UpdateDialog();
    }

    #endregion

    #region ==========Methods==========

    public void SetDialog(DialogueData[] dialogs)
    {
        //Init fields
        player_LD.enabled = false;
        system_LD.enabled = false;

        foreach (DialogueData dialog in dialogs)
        {
            switch (dialog.character)
            {
                case Character.Unknown or Character.Value:
                    player_LD.enabled = true;
                    break;
                case Character.System:
                    system_LD.enabled = true;
                    break;
            }
        }

        _dialogs = dialogs;
        _dialogIdx = 0;

        GameManager.Instance.Player.Controllable = false;

        OpenPanel();
        SetNextDialog();
    }

    public void UpdateDialog()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !UIManager.Instance.AnyPanelActivated)
        {
            if (_onTyping)
            {
                //타이핑 효과 중지하고, 현재 대사 전체 출력
                _onTyping = false;
                // StopAllCoroutines();
                textDialogue.text = _dialogs[_dialogIdx++].dialogue;
            }
            else if (_dialogIdx < _dialogs.Length)
            {
                SetNextDialog();
            }
            else
            {
                EndDialogue();
            }
        }

        if (_onTyping)
        {
            //Do Something
            Typing(_dialogs[_dialogIdx].dialogue).MoveNext();
        }
    }

    private void SetNextDialog()
    {
        DialogueData data = _dialogs[_dialogIdx];

        //Set an active current speaker
        Color inactive = new Color(0.6f, 0.6f, 0.6f);

        player_LD.rectTransform.localScale = new Vector3(0.9f, 0.9f, 1f);
        system_LD.rectTransform.localScale = new Vector3(0.9f, 0.9f, 1f);
        player_LD.color = inactive;
        system_LD.color = inactive;

        if (data.character == Character.Unknown)
        {
            player_LD.color = Color.clear;
            player_LD.rectTransform.localScale = Vector3.one;
        }
        else if (data.character == Character.Value)
        {
            player_LD.color = Color.white;
            player_LD.rectTransform.localScale = Vector3.one;
        }
        else if (data.character == Character.System)
        {
            system_LD.color = Color.white;
            system_LD.rectTransform.localScale = Vector3.one;
        }

        //Set Text
        textName.text = _dialogs[_dialogIdx].character switch
        {
            Character.Value => "밸류",
            Character.System => "시스템",
            Character.Unknown => "???",
            _ => throw new System.ArgumentOutOfRangeException()
        };

        textDialogue.text = string.Empty;
        // StartCoroutine(Typing(_dialogs[_dialogIdx].dialogue));

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.NextDialog);
        _onTyping = true;
    }

    private void EndDialogue()
    {
        // this.StopAllCoroutines();
        _onTyping = false;
        player_LD.enabled = false;
        system_LD.enabled = false;
        textDialogue.text = string.Empty;
        textName.text = string.Empty;
        _dialogIdx = 0;
        GameManager.Instance.Player.Controllable = true;
        ClosePanel();
    }

    private IEnumerator Typing(string dialog)
    {
        // _onTyping = true;

        for (int i = 0; i < dialog.Length; i++)
        {
            textDialogue.text += dialog[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        _dialogIdx++;
        // _onTyping = false;
    }

    #endregion
}