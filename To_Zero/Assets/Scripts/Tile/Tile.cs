using System;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    #endregion

    #region =====Unity Events=====

    protected virtual void OnEnable()
    {
        GameManager.Instance.OnRestart += OnRestart;
    }

    protected virtual void OnDisable()
    {
        GameManager.Instance.OnRestart -= OnRestart;
    }

    #endregion

    #region =====Methods=====

    public abstract void Init(string value);

    //보스 스테이지
    public virtual void Init()
    {
        OnRestart();
    }

    protected abstract void OnRestart();

    #endregion
}