using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Speaker
{
    public string characterName;
    public Image characterImage; 
   
}

[System.Serializable]
public struct DialogueData
{
    public int speakerIndex; // speaker 배열 순번
    public string name; // 캐릭터 이름
    [TextArea(3, 5)]
    public string dialogue; // 대사
}
public class DialogueSystem : MonoBehaviour
{
    #region ==========Fields==========
    [SerializeField]
    private Image imageDialog; // 공통 대화창 이미지
    [SerializeField]
    private TextMeshProUGUI textName; // 공통 캐릭터 이름
    [SerializeField]
    private TextMeshProUGUI textDialogue; // 공통 대사 출력 text

    [SerializeField]
    private Speaker[] speakers; // 캐릭터 정보 배열
    [SerializeField]
    private DialogueData[] dialogs; // 현재 분기의 대사 목록
    [SerializeField]
    private bool isAutoStart = true; // 자동시작여부

    private bool isFirst = true; // 최초 1회만 호출
    private int currentDialogIndex = -1; // 현재 대사순번
    private int currentSpeakerIndex = 0; // 화자의 배열순번
    private float typingSpeed = 0.1f;
    private bool isTypingEffect = false;
    #endregion

    #region ==========Unity Methods==========
    private void Awake()
    {
        Setup();
    }
    #endregion

    #region ==========Methods==========
    private void Setup()
    {
        // 대화 UI 초기 비활성화
        SetDialogueUIActive(false);

        for (int i = 0; i < speakers.Length; i++)
        {
            speakers[i].characterImage.gameObject.SetActive(true);
            Color color = speakers[i].characterImage.color;
            color.a = 0.2f;
            speakers[i].characterImage.color = color;
        }
    }

    public bool UpdateDialog()
    {
        if (isFirst == true)
        {
            Setup();
            if (isAutoStart) SetNextDialog();
            isFirst = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isTypingEffect == true)
            {
                //타이핑 효과 중지하고, 현재 대사 전체 출력
                isTypingEffect = false;
                StopCoroutine("OnTypingText");
                textDialogue.text = dialogs[currentDialogIndex].dialogue;
            }
            else if (dialogs.Length > currentDialogIndex + 1)
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
        // 이전 화자 비활성화
        if (currentDialogIndex >= 0)
        {
            SetSpeakerActive(currentSpeakerIndex, false);
        }

        // 다음 대사로 이동
        currentDialogIndex++;
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        // 현재 화자 활성화
        string speakerName = dialogs[currentDialogIndex].name;
        currentSpeakerIndex = FindSpeakerByName(speakerName);

        if (currentSpeakerIndex != -1)
        {
            // 현재 화자 활성화 (선명하게)
            SetSpeakerActive(currentSpeakerIndex, true);
        }
        // 공통 UI에 텍스트 설정
        textName.text = dialogs[currentDialogIndex].name;
        //textDialogue.text = dialogs[currentDialogIndex].dialogue;
        StartCoroutine("OnTypingText");
        // 대화 UI 활성화
        SetDialogueUIActive(true);
    }

    private int FindSpeakerByName(string name)
    {
        for (int i = 0; i < speakers.Length; i++)
        {
            if (speakers[i].characterName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1; // 찾지 못한 경우
    }

    private void SetSpeakerActive(int speakerIndex, bool isActive)
    {
        if (speakerIndex < 0 || speakerIndex >= speakers.Length) return;

        Color color = speakers[speakerIndex].characterImage.color;
        color.a = isActive ? 1.0f : 0.2f;
        speakers[speakerIndex].characterImage.color = color;
    }

    private void SetDialogueUIActive(bool isActive)
    {
        imageDialog.gameObject.SetActive(isActive);
        textName.gameObject.SetActive(isActive);
        textDialogue.gameObject.SetActive(isActive);
    }

    private void EndDialogue()
    {
        // 대화 UI 비활성화
        SetDialogueUIActive(false);

        // 모든 캐릭터 스프라이트 비활성화
        for (int i = 0; i < speakers.Length; i++)
        {
            speakers[i].characterImage.gameObject.SetActive(false);
        }
    }
    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        while (index < dialogs[currentDialogIndex].dialogue.Length)
        {
            textDialogue.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTypingEffect = false;
    }
    #endregion
}

