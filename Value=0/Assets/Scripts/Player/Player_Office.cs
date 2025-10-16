using System;
using System.Collections;
using UnityEngine;
using static GLOBAL;

public class Player_Office : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Configuration")]
    [SerializeField] private AnimationCurve easeOut;

    private bool _isMovable = true;
    private IInteractable interactable;

    #endregion

    #region =====Unity Events=====

    private void Update()
    {
        InputHandler();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out interactable)) return;

        interactable.Notify(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out interactable)) return;
        interactable.Notify(false);
        interactable = null;
    }

    #endregion

    #region =====Methods=====

    private void InputHandler()
    {
        if (UIManager.Instance.AnyPanelOpen) return;
        Vector2 dir = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2.right;
        if (dir != Vector2.zero) Move(dir);

        if (interactable != null && Input.GetKeyDown(KeyCode.Space)) interactable.Interact();
    }

    private void Move(Vector2 dir)
    {
        if (!_isMovable) return;

        _isMovable = false;
        Vector2 pos = (Vector2)this.transform.position + dir;
        
        if (dir == Vector2.right) spriteRenderer.flipX = true;
        else if (dir == Vector2.left) spriteRenderer.flipX = false;
        StartCoroutine(Crtn_Move(pos));
    }

    private IEnumerator Crtn_Move(Vector2 pos)
    {
        Vector2 startPos = this.transform.position;
        float t = 0f;
        animator.SetTrigger(Animator.StringToHash("Move"));

        SoundManager.Instance.PlayOneShot(SFX_ID.PlayerMove);
        while ((Vector2)this.transform.position != pos)
        {
            this.transform.position = Vector2.Lerp(startPos, pos, Mathf.Clamp01(easeOut.Evaluate(t / 0.05f)));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = pos;
        _isMovable = true;
    }

    #endregion
}