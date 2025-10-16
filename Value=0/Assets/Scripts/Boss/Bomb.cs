using System;
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float explosionDuration = 0.3f;

    


    public void Explode()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }
        
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }
}
