using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField]
    private DialogueSystem dialogueSystem01;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        dialogueSystem01.UpdateDialog();
    }

   
}
