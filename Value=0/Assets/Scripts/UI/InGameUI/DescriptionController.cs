using UnityEngine;

public class DescriptionController : MonoBehaviour
{
    [SerializeField] private InGameUIController inGameUIController;



    #region ========== ButtonEvent =========

    public void OnClick_Exit()
    {
        inGameUIController.SetActive_DescriptionPanel(false);
    }

    public void OnClick_LeftArrow()
    {
        //영상 변경
        //제목 텍스트 변경
        //설명 텍스트 변경
    }

    public void OnClick_RightArrow()
    {
        //영상 변경
        //제목 텍스트 변경
        //설명 텍스트 변경
    }

    #endregion
}
