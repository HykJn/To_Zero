using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("Configuration")]
    [SerializeField] protected Vector2 startPoint;
    [SerializeField] protected Vector2 endPoint;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    public virtual void Init(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    #endregion
}