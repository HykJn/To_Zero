using UnityEngine;

public class _TEMP : MonoBehaviour
{
    #region ==========Properties==========

    #endregion

    #region ==========Fields==========

    #endregion

    #region ==========Unity Methods==========

    #endregion

    #region ==========Methods==========

    public void OnClickSave()
    {
        DataManager.Save();
    }

    public void OnClickLoad()
    {
        DataManager.Load();
    }

    #endregion
}