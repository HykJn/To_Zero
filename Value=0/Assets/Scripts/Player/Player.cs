using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ==========Properties==========
    public int StartNumber { get; set; }
    public int Moves { get; set; }
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
        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;
        if (dir != Vector3Int.zero)
        {
            Move(dir);
        }

        //Restart
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    private void Move(Vector3 dir)
    {
        if (!IsMovable) return;
        if (Moves <= 0)
        {
            Die();
            return;
        }
        if (CheckWall(dir)) return;

        this.transform.position += dir;
        GameManager.Instance.SwapTiles();

        CheckTile();
    }

    private bool CheckWall(Vector3 dir)
    {
        return Physics2D.Raycast(this.transform.position + dir, Vector3.forward, 5f, LayerMask.GetMask("Wall"));
    }

    private void CheckTile()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector3.forward, 5f, LayerMask.GetMask("Tile"));
        if (hit)
        {
            OperationTile tile = hit.transform.parent.GetComponent<OperationTile>();
            switch (tile.Operator)
            {
                case Operator.Portal:
                    if (StartNumber == 0) GameManager.Instance.Transition(EventID.NextStage);
                    else Die();
                    break;
                case Operator.Add:
                    StartNumber += tile.Value;
                    break;
                case Operator.Sub:
                    StartNumber -= tile.Value;
                    break;
                case Operator.Mul:
                    StartNumber *= tile.Value;
                    break;
                case Operator.Div:
                    StartNumber /= tile.Value;
                    break;
                case Operator.Equal:
                    if (StartNumber != tile.Value) Die();
                    break;
                case Operator.Not:
                    if (StartNumber == tile.Value) Die();
                    break;
                case Operator.Greater:
                    if (StartNumber <= tile.Value) Die();
                    break;
                case Operator.Less:
                    if (StartNumber >= tile.Value) Die();
                    break;
            }
            Moves--;
            moves = Moves;
            startNumber = StartNumber;
        }
    }

    public void Die()
    {
        GameManager.Instance.Transition(EventID.PlayerDie);
    }

    private void Restart()
    {
        if (!IsMovable) return;
        GameManager.Instance.Restart();
    }
    #endregion
}
