using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ==========Properties==========
    public int Value { get; set; }
    public int Moves { get; set; }
    public bool IsMovable { get; set; } = true;
    public bool Controllable { get; set; } = true;
    #endregion

    #region ==========Fields==========
    [SerializeField] private AnimationCurve easeOutCurve;

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
            Die(EventID.PlayerDieByDrone);
        }
    }
    #endregion

    #region ==========Methods==========
    private void InputHandler()
    {
        if (UIManager.Instance.AnyPanelActivated || !Controllable) return;
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
            dir = Vector3.left;
            if (!onHold) this.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D))
        {
            dir = Vector3.right;
            if (!onHold) this.GetComponent<SpriteRenderer>().flipX = false;
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
            Die(EventID.PlayerDieByMoves);
            return;
        }
        if (CheckBox(this.transform.position + dir) || !CheckMovable(this.transform.position + dir)) return;

        //this.transform.position += dir;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector3.forward, 5f, LayerMask.GetMask("Tile"));
        if (hit)
        {
            hit.transform.parent.GetComponent<OperationTile>().OnPlayer = false;
        }

        //if (dir == Vector3.left) this.GetComponent<SpriteRenderer>().flipX = true;
        //else if (dir == Vector3.right) this.GetComponent<SpriteRenderer>().flipX = false;

        SoundManager.Instance.PlayOneShot_SFX(SFXID.PlayerMove);

        this.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Crtn_Move(this.transform.position, this.transform.position + dir));
        this.GetComponent<Animator>().SetTrigger("Move");

        //CheckTile();
        //CheckBox(this.transform.position + dir);
    }

    private IEnumerator Crtn_Move(Vector3 from, Vector3 to)
    {
        IsMovable = false;
        float t = 0;
        while (Vector3.Distance(this.transform.position, to) >= 0.05f)
        {
            this.transform.position = Vector3.Lerp(from, to, easeOutCurve.Evaluate(t / 0.06f));
            t += Time.deltaTime;
            yield return null;
        }
        this.transform.position = to;
        IsMovable = true;

        GameManager.Instance.SwapTiles();
        GameManager.Instance.MoveDrone();

        CheckTile();
        CheckBox(to + (to - from).normalized);
        this.GetComponent<Collider2D>().enabled = true;
    }

    private void HoldBox()
    {
        if (box == null) return;
        SoundManager.Instance.Play_SFX(SFXID.PlayerHoldBox);
        IsMovable = false;

        Vector3[] wasd = { Vector3.up, Vector3.left, Vector3.down, Vector3.right };
        foreach (Vector3 dir in wasd)
        {
            if (!CheckMovable(box.transform.position + dir)) continue;

            OperationTile tile = box.GetComponent<Box>().GetTileBelow(dir);
            if (tile == null || tile.Operator == Operator.Portal) continue;
            box.GetComponent<Box>().SetPreview(dir);
        }

        onHold = true;
        box.transform.localScale = new(1.1f, 1.1f, 1f);

        this.GetComponent<Animator>().SetBool("HoldBox", true);
    }

    private void UnHoldBox()
    {
        SoundManager.Instance.Play_SFX(SFXID.PlayerUnholdBox);
        onHold = false;
        box.transform.localScale = Vector3.one;
        if (box != null) box.GetComponent<Box>().ClearPreview();

        this.GetComponent<Animator>().SetBool("HoldBox", false);
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
        OperationTile tile = box.GetComponent<Box>().GetTileBelow(dir);
        if (tile == null || tile.Operator == Operator.Portal) return;

        box.GetComponent<Box>().GetTileBelow().GetComponent<Animator>().enabled = true;
        box.transform.position += dir;
        box.GetComponent<Box>().GetTileBelow().GetComponent<Animator>().enabled = false;

        box.GetComponent<Box>().UpdateValue();
        UnHoldBox();
        box.GetComponent<Box>().Selected = false;
        box = null;

        Moves--;
        GameManager.Instance.SwapTiles();
        GameManager.Instance.MoveDrone();


        this.GetComponent<Animator>().SetTrigger("MoveBox");
        this.GetComponent<Animator>().SetBool("HoldBox", false);
    }

    private bool CheckMovable(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos + Vector3.back, Vector3.forward);
        if (hit) return hit.collider.gameObject.layer == 10;
        return false;
    }

    private void CheckTile()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position + Vector3.back, Vector3.forward, 5f, LayerMask.GetMask("Tile"));
        if (hit)
        {
            OperationTile tile = hit.transform.parent.GetComponent<OperationTile>();
            switch (tile.Operator)
            {
                case Operator.Portal:
                    if (Value == 0) GameManager.Instance.Transition(EventID.NextStage);
                    else Die(EventID.PlayerDieByMoves);
                    break;
                case Operator.Add:
                    Value += tile.Value;
                    break;
                case Operator.Sub:
                    Value -= tile.Value;
                    break;
                case Operator.Mul:
                    Value *= tile.Value;
                    break;
                case Operator.Div:
                    Value /= tile.Value;
                    break;
                case Operator.Equal:
                    if (Value != tile.Value) Die(EventID.PlayerDieByMoves);
                    break;
                case Operator.Not:
                    if (Value == tile.Value) Die(EventID.PlayerDieByMoves);
                    break;
                case Operator.Greater:
                    if (Value <= tile.Value) Die(EventID.PlayerDieByMoves);
                    break;
                case Operator.Less:
                    if (Value >= tile.Value) Die(EventID.PlayerDieByMoves);
                    break;
            }
            Moves--;
            tile.OnPlayer = true;
        }
    }

    private bool CheckBox(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos + Vector3.back, Vector3.forward, 5f, LayerMask.GetMask("Box"));
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

    public void Die(EventID diedBy)
    {
        GameManager.Instance.Transition(diedBy);
    }

    private void Restart()
    {
        if (onHold && !IsMovable)
        {
            onHold = false;
            box.transform.localScale = Vector3.one;
            if (box != null) box.GetComponent<Box>().ClearPreview();
            box = null;
            this.GetComponent<Animator>().SetBool("HoldBox", false);
        }
        else if (!IsMovable) return;
        GameManager.Instance.Restart();
        //TODO: Modify later
        //this.GetComponent<SpriteRenderer>().flipX = false;
    }
    #endregion

    #region ==========For Animation Event==========
    public void AnimationMoveLock()
    {
        IsMovable = !IsMovable;
    }
    #endregion
}
