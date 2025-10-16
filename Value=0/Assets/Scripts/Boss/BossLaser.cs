using System.Collections;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    #region ==========Fields===========

    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Collider2D col;
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private Color attackColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float attackEffectDuration = 0.3f;

    //private bool _isActive = false;
    #endregion\

    #region ==========Unity Methods==========

  
    #endregion

    #region ==========Methods==========

    public void ShowWarning(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        spriteRenderer.color = warningColor;
        //col.enabled = false;
        //_isActive = col.enabled;

        StartCoroutine(BlinkWarning());
    }

    public void Attack()
    {
        if (!gameObject.activeSelf) return;

        StopAllCoroutines();
        spriteRenderer.color = attackColor;
        //col.enabled = true;
        //_isActive |= col.enabled;

        StartCoroutine(DestroyDelay(attackEffectDuration));
    }

    private IEnumerator BlinkWarning()
    {
        float elapsed = 0f;

        while (true)
        {
            float alpha = Mathf.PingPong(elapsed * 4f, 0.5f);
            Color color = warningColor;
            color.a = alpha;
            spriteRenderer.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DestroyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectManager.Instance.ReleaseObject(gameObject);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(!_isActive) return;

    //    if(other.CompareTag("Player"))
    //    {
    //        Debug.Log("레이저와 충돌");
    //        BossManager.Instance?.DamagePlayer();
    //    }
    //}

    #endregion
}
