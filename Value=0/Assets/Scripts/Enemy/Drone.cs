using UnityEngine;

public class Drone : MonoBehaviour
{
    #region ==========Properties==========
    #endregion

    #region ==========Fields==========
    [SerializeField] private Vector2 start;
    [SerializeField] private Direction direction;
    private Direction curDirection;
    [SerializeField] private int steps;
    private int curSteps;
    [SerializeField] private GameObject scanner;
    #endregion

    #region ==========Unity Methods==========

    #endregion

    #region ==========Methods==========
    public void Init(Vector2 start, Direction direction, int steps)
    {
        this.start = start;
        this.direction = direction;
        this.steps = steps;

        Init();
    }

    public void Init()
    {
        this.transform.position = start;
        curSteps = steps;
        curDirection = direction;
        this.GetComponent<Animator>().SetFloat("Direction", (int)curDirection);

        scanner.transform.localPosition = direction switch
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
        if (curSteps == 0)
        {
            curSteps--;
            return;
        }
        else if (curSteps == -1)
        {
            curDirection = (Direction)(-(int)curDirection);
            this.GetComponent<Animator>().SetFloat("Direction", (int)curDirection);
            curSteps = steps;
            scanner.transform.localPosition = -scanner.transform.localPosition;
            return;
        }

        this.transform.position += curDirection switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => throw new System.ArgumentOutOfRangeException()
        };
        curSteps--;
    }
    #endregion

    public enum Direction
    {
        Up = 1, Down = -1, Left = 2, Right = -2
    }
}