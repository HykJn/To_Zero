using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ==========Properties==========
    public int StartNumber => startNumber;
    public int Moves => moves;
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
    }

    private void Move(Vector3Int dir)
    {
        this.transform.position += dir;

        CheckTile();
    }

    private void CheckTile()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Tile")))
        {
            LogicTile tile = hit.collider.GetComponent<LogicTile>();
            startNumber = tile.Operator switch
            {
                Operator.Add => startNumber + tile.Value,
                Operator.Sub => startNumber - tile.Value,
                Operator.Mul => startNumber * tile.Value,
                Operator.Div => startNumber / tile.Value,
                _ => throw new ArgumentException($"Unknown operator: {tile.Operator}"),
            };
            moves--;
        }
        else
        {
            Debug.Log("Can't find any tile");
        }
    }
    #endregion
}
