using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : Panel
{
    #region ==========Properties==========

    public TMP_Text LoadingText => loadingText;
    public Image LoadingFill => loadingFill;

    #endregion

    #region ==========Fields==========

    [Header("Components")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Image loadingFill;

    #endregion
}