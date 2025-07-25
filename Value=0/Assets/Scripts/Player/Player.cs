using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ==========Properties==========
    public int StartNumber { get => startNumber; set => startNumber = value; }
    public int Moves { get => moves; set => moves = value; }
    public bool IsMovable { get; set; } = true;
    #endregion

    #region ==========Fields==========
    [SerializeField] private int startNumber;
    [SerializeField] private int moves;
    #endregion

    #region ==========Unity==========
    private void Update()
    {
        InputHandler();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Scanner"))
        {
            Die();
        }
    }
    #endregion

    #region ==========Methods==========
    private void InputHandler()
    {
        //Moves
        Vector3Int dir = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3Int.forward;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3Int.back;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3Int.right;
        if (dir != Vector3Int.zero)
        {
            Move(dir);
        }

        //Restart
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    private void Move(Vector3Int dir)
    {
        if (!IsMovable) return;
        if (moves <= 0)
        {
            Die();
            return;
        }
        if (CheckWall(dir)) return;

        this.transform.position += dir;

        CheckTile();
    }

    private bool CheckWall(Vector3Int dir)
    {
        return Physics.Raycast(this.transform.position + dir + Vector3.up, Vector3.down, 10f, LayerMask.GetMask("Wall"));
    }

    private void CheckTile()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Tile")))
        {
            LogicTile tile = hit.collider.GetComponent<LogicTile>();
            if (tile.Operator == Operator.Portal)
            {
                if (startNumber == 0)
                {
                    //GameManager.Instance.Stage++;
                    GameManager.Instance.Transition(EventID.NextStage);
                }
                else
                {
                    Die();
                }
            }
            else
            {
                startNumber = tile.Operator switch
                {
                    Operator.Add => startNumber + tile.Value,
                    Operator.Sub => startNumber - tile.Value,
                    Operator.Mul => startNumber * tile.Value,
                    Operator.Div => startNumber / tile.Value,
                    //_ => throw new ArgumentException($"Unknown operator: {tile.Operator}"),
                    _ => startNumber,
                };
            }
            moves--;
        }
        else
        {
            Debug.Log("Can't find any tile");
        }
    }

    public void Die()
    {
        //TODO: Implement Die logic
        //Restart();
        GameManager.Instance.Transition(EventID.PlayerDie);
    }

    private void Restart()
    {
        if (!IsMovable) return;
        GameManager.Instance.Restart();
    }
    #endregion
}
