using UnityEngine;
using static GlobalDefines;

public class Panel : MonoBehaviour
{
    public virtual void OpenPanel()
    {
        UIManager.Instance.OpenPanel.Push(this);
        this.gameObject.SetActive(true);

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.PanelOpen);
    }

    public virtual void ClosePanel()
    {
        if (UIManager.Instance.OpenPanel.Peek() == this)
            UIManager.Instance.OpenPanel.Pop();
        this.gameObject.SetActive(false);

        SoundManager.Instance.Play_UI_SFX(UI_SFX_ID.PanelClose);
    }
}