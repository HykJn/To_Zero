using System;
using UnityEngine;

public class Pix : MonoBehaviour, IInteractable
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("References")]
    [SerializeField] private GameObject mark;

    #endregion

    #region =====Unity Events=====

    private void Update()
    {
        _Notify();
    }

    #endregion

    #region =====Methods=====

    private void _Notify()
    {
        switch (SequanceManager.Chapter)
        {
            case 2 when SequanceManager.Stage == 6:
            {
                mark.SetActive(SequanceManager.LastDialog != 14);
                break;
            }
            case 2 when SequanceManager.Stage == 7:
            {
                mark.SetActive(SequanceManager.LastDialog != 20);
                break;
            }
            case 3 when SequanceManager.Stage == 12:
            {
                mark.SetActive(SequanceManager.LastDialog != 23);
                break;
            }
            case 3 when SequanceManager.Stage == 13:
            {
                mark.SetActive(SequanceManager.LastDialog != 30);
                break;
            }
            case 4 when SequanceManager.Stage == 16:
            {
                mark.SetActive(SequanceManager.LastDialog != 33);
                break;
            }
            case 4 when SequanceManager.Stage == 17:
            {
                mark.SetActive(SequanceManager.LastDialog != 40);
                break;
            }
            case 5:
            {
                mark.SetActive(SequanceManager.LastDialog != 50);
                break;
            }
            default:
                mark.SetActive(false);
                break;
        }
    }

    public void Notify(bool flag) { }

    public void Interact()
    {
        switch (SequanceManager.Chapter)
        {
            case 2 when SequanceManager.Stage == 6:
            {
                if (SequanceManager.LastDialog == 14) return;
                UIManager.Instance.DialogPanel.SetDialog(14);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
            }
            case 2 when SequanceManager.Stage == 7:
            {
                if (SequanceManager.LastDialog == 20) return;
                UIManager.Instance.DialogPanel.SetDialog(20);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
            }
            case 3 when SequanceManager.Stage == 12:
            {
                if (SequanceManager.LastDialog == 23) return;
                UIManager.Instance.DialogPanel.SetDialog(23);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
            }
            case 3 when SequanceManager.Stage == 13:
            {
                if (SequanceManager.LastDialog == 30) return;
                UIManager.Instance.DialogPanel.SetDialog(30);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
            }
            case 4 when SequanceManager.Stage == 16:
            {
                if (SequanceManager.LastDialog == 33) return;
                UIManager.Instance.DialogPanel.SetDialog(33);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
            }
            case 4 when SequanceManager.Stage == 17:
            {
                if (SequanceManager.LastDialog == 40) return;
                UIManager.Instance.DialogPanel.SetDialog(40);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
            }
            case 5 when SequanceManager.LastDialog == 50:
                return;
            case 5:
                UIManager.Instance.DialogPanel.SetDialog(50);
                UIManager.Instance.DialogPanel.StartDialog();
                break;
        }
    }

    #endregion
}