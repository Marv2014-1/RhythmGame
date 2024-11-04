using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damageAmount = 10;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Retrieve the damage amount from the parent MeleeAttack component
            MeleeAttack meleeAttack = GetComponentInParent<MeleeAttack>();
            if (meleeAttack != null)
            {
                damageAmount = meleeAttack.GetAttackDamage();
            }
            // Assume the player has a component `PlayerHealth` to handle damage
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Player took damage from skeleton hitbox.");
            }
        }
    }
}

