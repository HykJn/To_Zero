using UnityEngine;

public class Controller : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StageTransition.Instance.CurrentStage++;
        }
    }
}
