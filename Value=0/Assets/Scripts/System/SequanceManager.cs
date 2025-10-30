using System;
using UnityEngine;
using static GLOBAL;

public static class SequanceManager
{
    #region =====Properties=====

    public static SceneID SceneID
    {
        get => _sceneID;
        set
        {
            _sceneID = value;
            switch (value)
            {
                case SceneID.Title:
                    SoundManager.Instance.Play(BGM_ID.Title);
                    break;
                case SceneID.Office:
                    SoundManager.Instance.Play(BGM_ID.Office);
                    break;
                case SceneID.Matrix:
                    SoundManager.Instance.Play(BGM_ID.Matrix);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public static int Chapter { get; set; }
    public static int Stage { get; set; }
    public static int LastDialog { get; set; }

    #endregion

    #region =====Fields=====

    private static SceneID _sceneID = SceneID.Title;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    #endregion
}