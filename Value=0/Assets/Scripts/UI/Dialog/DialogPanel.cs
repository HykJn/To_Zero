using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogPanel : MonoBehaviour, IPanel
{
    private Dictionary<int, DialogGroup> dialogs;

    private void Awake()
    {
        LoadDialogs();
    }

    public void Open()
    {
        UIManager.Instance.OpenPanel(this);
    }

    public void Close()
    {
        UIManager.Instance.ClosePanel();
    }

    public void ForceClose()
    {
        throw new System.NotImplementedException();
    }

    private void LoadDialogs() => dialogs = Resources.LoadAll<DialogGroup>("Dialogs").ToDictionary(x => x.DialogID);
}