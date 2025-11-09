using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog/New Dialog")]
public class DialogGroup : ScriptableObject
{
    public int DialogID => dialogID;
    public Dialog[] Dialogs => dialogs;

    [SerializeField] private int dialogID;
    [SerializeField] private Dialog[] dialogs;
}