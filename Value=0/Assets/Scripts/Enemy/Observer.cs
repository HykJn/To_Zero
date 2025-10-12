using UnityEngine;

public class Observer : Enemy
{
    #region =====Properties=====
    
    public bool IsMovable { get; set; }
    
    #endregion

    #region =====Fields=====

    [Header("Configuration")]
    [SerializeField] private Vector2 startPoint, endPoint;

    #endregion

    #region =====Unity Events=====

    #endregion

    #region =====Methods=====

    private void Move()
    {
        
    }

    private void OnHacked()
    {
        
    }

    public void Init(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    private void OnRestart()
    {
        
    }

    #endregion
}
