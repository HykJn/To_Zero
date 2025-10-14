using System;
using UnityEngine;

public class Player_Office : MonoBehaviour
{
    #region =====Properties=====

    #endregion

    #region =====Fields=====

    [Header("Configuration")]
    [SerializeField] private float speed = 1.5f;

    [SerializeField] private IInteractable interactable;

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
        Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized);

        if (interactable != null && Input.GetKeyDown(KeyCode.Space)) interactable.Interact();
    }

    private void Move(Vector2 dir)
    {
        this.transform.Translate(speed * Time.deltaTime * dir);
    }

    #endregion
}