using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BossTargetUI : MonoBehaviour
{
    public Transform BossHead;
    public TMP_Text text_target;
    public Vector3 offset = new Vector3(-6.0f, 2.0f, 0);

    private void Update()
    { 
        if (BossHead != null && text_target != null)
        {
            text_target.transform.position = BossHead.position + offset;
        }
    }


}
