using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    private Vector2 _startPoint;
    private Vector2? _endPoint;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public virtual void Init(Vector2 startPoint, Vector2? endPoint = null)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;
    }

    #endregion
}