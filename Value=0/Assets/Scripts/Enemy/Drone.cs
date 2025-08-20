using UnityEngine;

public class Drone : MonoBehaviour
{
    #region ==========Fields==========
    [Header("Components"), SerializeField] private Animator animator;
    [Header("References"), SerializeField] private GameObject scanner;
    private Vector2 _start;
    private Direction _direction, _curDirection;
    private int _steps, _curSteps;
    #endregion
    
    #region ==========Methods==========
    public void Init(Vector2 start, Direction direction, int steps)
    {
        _start = start;
        _direction = direction;
        _steps = steps;

        Restart();
    }

    public void Restart()
    {
        this.transform.position = _start;
        _curSteps = _steps;
        _curDirection = _direction;
        animator.SetFloat(Animator.StringToHash("Direction"), (int)_curDirection);

        scanner.transform.localPosition = _direction switch
        {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            _ => throw new System.InvalidOperationException()
        };
    }
    
    public void Move()
    {
        if (_curSteps == 0)
        {
            _curSteps--;
        }
        else if (_curSteps == -1)
        {
            _curDirection = (Direction)(-(int)_curDirection);
            animator.SetFloat(Animator.StringToHash("Direction"), (int)_curDirection);
            _curSteps = _steps;
            scanner.transform.localPosition = -scanner.transform.localPosition;
            return;
        }

        this.transform.position += _curDirection switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => throw new System.ArgumentOutOfRangeException()
        };
        _curSteps--;
    }
    #endregion

    public enum Direction
    {
        Up = 1, Down = -1, Left = 2, Right = -2
    }
}