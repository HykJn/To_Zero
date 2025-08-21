using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] private DialogPanel dialogueSystem01;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        dialogueSystem01.UpdateDialog();
    }
}