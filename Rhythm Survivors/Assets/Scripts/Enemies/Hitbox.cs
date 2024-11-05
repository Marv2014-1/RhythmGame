using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damageAmount = 10;
    private Animator animator;
    public float knockbackForce = 5f;
    protected Transform playerTransform;


    private void Start()
    {
        animator = GetComponent<Animator>();
        
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MeleeAttack meleeAttack = GetComponentInParent<MeleeAttack>();
        if (meleeAttack != null)
        {
            damageAmount = meleeAttack.GetAttackDamage();
        }

        try
        {
            // Ensure you have tagged the player GameObject with "Player"
            if (collision.CompareTag("Player"))
            {
                Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageAmount, knockbackDirection, knockbackForce);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
}

