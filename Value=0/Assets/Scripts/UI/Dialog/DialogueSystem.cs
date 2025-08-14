using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueData
{
    public Character character;
    [TextArea(3, 5)]
    public string dialogue; // 대사
}
public class DialogueSystem : MonoBehaviour
{
    #region ==========Fields==========
    [SerializeField] private Image player_LD, system_LD;

    [SerializeField] private GameObject dialogPanel;

    [SerializeField]
    private Image imageDialog; // 공통 대화창 이미지
    [SerializeField]
    private TextMeshProUGUI textName; // 공통 캐릭터 이름
    [SerializeField]
    private TextMeshProUGUI textDialogue; // 공통 대사 출력 text

    [SerializeField]
    private DialogueData[] dialogs; // 현재 분기의 대사 목록

    [SerializeField] private int dialogIdx = 0; // 현재 대사순번
    [SerializeField] private float typingSpeed = 0.1f;
    private bool onTyping = false;
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
        player_LD.enabled = false;
        system_LD.enabled = false;

        foreach (DialogueData dialog in dialogs)
        {
            if (dialog.character == Character.Unknown || dialog.character == Character.Value) player_LD.enabled = true;
            else if (dialog.character == Character.System) system_LD.enabled = true;
        }
        this.dialogs = dialogs;
        dialogIdx = 0;
        this.gameObject.SetActive(true);
        GameObject.FindWithTag("Player").GetComponent<Player>().Controllable = false;

        SetNextDialog();
    }

    public bool UpdateDialog()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !UIManager.Instance.AnyPanelActivated)
        {
            if (onTyping == true)
            {
                //타이핑 효과 중지하고, 현재 대사 전체 출력
                onTyping = false;
                StopAllCoroutines();
                textDialogue.text = dialogs[dialogIdx++].dialogue;
            }
            else if (dialogIdx < dialogs.Length)
            {
                SetNextDialog();
            }
            else
            {
                EndDialogue();
            }
        }
        return false;
    }

    private void SetNextDialog()
    {
        DialogueData data = dialogs[dialogIdx];

        //Set active current speaker
        Color inactive = new Color(0.6f, 0.6f, 0.6f);

        player_LD.rectTransform.localScale = new Vector3(0.9f, 0.9f, 1f);
        system_LD.rectTransform.localScale = new Vector3(0.9f, 0.9f, 1f);
        player_LD.color = inactive;
        system_LD.color = inactive;

        if (data.character == Character.Unknown)
        {
            player_LD.color = new Color(1, 1, 1, 0);
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
        textName.text = dialogs[dialogIdx].character switch
        {
            Character.Value => "밸류",
            Character.System => "시스템",
            Character.Unknown => "???",
            _ => throw new System.Exception()
        };

        textDialogue.text = string.Empty;
        StartCoroutine(Typing(dialogs[dialogIdx].dialogue));
    }

    private void EndDialogue()
    {
        this.StopAllCoroutines();
        onTyping = false;
        player_LD.enabled = false;
        system_LD.enabled = false;
        textDialogue.text = string.Empty;
        textName.text = string.Empty;
        dialogIdx = 0;
        GameObject.FindWithTag("Player").GetComponent<Player>().Controllable = true;
        dialogPanel.SetActive(false);
    }

    private IEnumerator Typing(string dialog)
    {
        onTyping = true;

        for (int i = 0; i < dialog.Length; i++)
        {
            textDialogue.text += dialog[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        dialogIdx++;
        onTyping = false;
    }
    #endregion
}

