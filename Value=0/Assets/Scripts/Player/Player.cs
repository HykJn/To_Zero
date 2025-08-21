using System;
using System.Collections;
using UnityEngine;
using static GlobalDefines;

public class Player : MonoBehaviour
{
    #region ==========Events==========

    public event Action OnPlayerMove;

    #endregion

    #region ==========Properties==========

    public int Value { get; set; }
    public int Moves { get; set; }
    public bool IsMovable { get; set; } = true;
    public bool Controllable { get; set; } = true;

    #endregion

    #region ==========Fields==========

    [SerializeField] private AnimationCurve easeOutCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col;

    private Box _box;
    private bool _onHold = false;

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
            if (!_onHold) spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D))
        {
            dir = Vector3.right;
            if (!_onHold) spriteRenderer.flipX = false;
        }

        if (dir != Vector3Int.zero)
        {
            Move(dir);
        }

        //Move Box
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_onHold) ReleaseBox();
            else HoldBox();
        }

        if (_onHold)
        {
            MoveBox();
        }

        //Restart
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    private void Move(Vector3 dir)
    {
        if (!IsMovable) return;
        
        
    }

    private IEnumerator Crtn_Move(Vector3 from, Vector3 to)
    {
        SoundManager.Instance.PlayOneShot_SFX(SFX_ID.PlayerMove);
        animator.SetTrigger(Animator.StringToHash("Move"));

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

        CheckTile();
        CheckBox(to + (to - from).normalized);
        col.enabled = true;
    }

    private void HoldBox()
    {
        if (!_box) return;

        SoundManager.Instance.Play_SFX(SFX_ID.PlayerHoldBox);
        animator.SetBool(Animator.StringToHash("HoldBox"), true);

        IsMovable = false;
        _onHold = true;
        _box.SetPreview(true);
    }

    private void ReleaseBox()
    {
        SoundManager.Instance.Play_SFX(SFX_ID.PlayerReleaseBox);
        animator.SetBool(Animator.StringToHash("HoldBox"), false);

        IsMovable = true;
        _onHold = false;
        if (_box) _box.SetPreview(false);
    }

    private void MoveBox()
    {
        if (!_onHold) return;

        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;
        if (dir == Vector3.zero) return;

        if (!_box.Move(dir)) return;
        ReleaseBox();
        _box = null;

        Moves--;
        OnPlayerMove?.Invoke();

        animator.SetTrigger(Animator.StringToHash("MoveBox"));
        animator.SetBool(Animator.StringToHash("HoldBox"), false);
    }
    public void Die(EventID diedBy)
    {
        GameManager.Instance.Transition(diedBy);
    }

    private void Restart()
    {
        if (_onHold && !IsMovable)
        {
            ReleaseBox();
            if (_box) _box.SetPreview(false);
            _box = null;
            animator.SetBool(Animator.StringToHash("HoldBox"), false);
        }
        else if (!IsMovable) return;

        _box = null;
        GameManager.Instance.Restart();
    }

    #endregion

    #region ==========For Animation Event==========

    public void AnimationMoveLock()
    {
        IsMovable = !IsMovable;
    }

    #endregion
}