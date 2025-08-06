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
    private int startNumber;
    private int moves;
    private GameObject box;
    private bool onHold;
    #endregion

    #region ==========Unity==========
    private void Update()
    {
        InputHandler();
    }

    private void OnTriggerEnter2D(Collider2D other)
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
        //Mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //TODO; Do something
        }

        //Moves
        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A))
        {
            this.GetComponentInChildren<SpriteRenderer>().flipX = true;
            dir = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D))
        {
            this.GetComponentInChildren<SpriteRenderer>().flipX = false;
            dir = Vector3.right;
        }
        if (dir != Vector3Int.zero)
        {
            Move(dir);
        }

        //Move Box
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (onHold) UnHoldBox();
            else HoldBox();
        }
        if (onHold)
        {
            MoveBox();
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
        if (CheckBox(this.transform.position + dir) || !CheckMovable(this.transform.position + dir)) return;

        this.transform.position += dir;
        this.GetComponentInChildren<Animator>().SetTrigger("Move");
        GameManager.Instance.SwapTiles();

        CheckTile();
        CheckBox(this.transform.position + dir);
    }

    private void HoldBox()
    {
        if (box == null) return;
        IsMovable = false;

        Vector3[] wasd = { Vector3.up, Vector3.left, Vector3.down, Vector3.right };
        foreach (Vector3 dir in wasd)
        {
            if (CheckMovable(box.transform.position + dir))
                box.GetComponent<Box>().SetPreview(dir);
        }

        onHold = true;
        //TODO: Implement UI interaction
    }

    private void UnHoldBox()
    {
        IsMovable = true;
        onHold = false;
        if (box != null) box.GetComponent<Box>().ClearPreview();
    }

    private void MoveBox()
    {
        if (!onHold) return;

        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;

        if (dir == Vector3.zero || !CheckMovable(box.transform.position + dir)) return;
        box.transform.position += dir;
        Moves--;
        box.GetComponent<Box>().UpdateValue();
        box.GetComponent<Box>().Selected = false;
        box.GetComponent<Box>().ClearPreview();
        box = null;
        onHold = false;
        IsMovable = true;


        //TODO: Implement UI interaction
    }

    private bool CheckMovable(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.forward);
        if (hit) return hit.collider.gameObject.layer == 10;
        return false;
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

    private bool CheckBox(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.forward, 5f, LayerMask.GetMask("Box"));
        if (hit)
        {
            if (box != null) box.GetComponent<Box>().Selected = false;
            box = hit.collider.transform.parent.gameObject;
            box.GetComponent<Box>().Selected = true;
            return true;
        }
        if (box != null) box.GetComponent<Box>().Selected = false;
        box = null;
        return false;
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
