using System.Collections;
using UnityEngine;

public class BossHitEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeAmount = 0.2f;

    [SerializeField] private GameObject targetText;

    private Vector3 originalPosition;
    private Color originalColor;

    private void Start()
    {
        originalPosition = transform.position;
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(HitCoroutine());
            Debug.Log("���� ����Ʈ ȣ���");
        }
    }

    public void PlayerHitEffect()
    {
         StartCoroutine(HitCoroutine());
         Debug.Log("���� ����Ʈ ȣ���");  
        
    }

    private IEnumerator HitCoroutine()
    {
        targetText.SetActive(false);
        spriteRenderer.color = Color.red;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-shakeAmount, shakeAmount);
            transform.position = new Vector3(x, originalPosition.y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        spriteRenderer.color = originalColor;
        targetText.SetActive(true);
    }
}
