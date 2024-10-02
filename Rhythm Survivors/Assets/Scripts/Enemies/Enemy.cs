using UnityEngine;

// Make sure any class that inherits from Enemy requires a Collider and a Rigidbody
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField]
    protected int maxHealth = 100;
    
    protected int currentHealth;

    [Header("Death Settings")]
    [SerializeField]
    protected GameObject deathEffect; // Particle effect or animation on death

    // Initialization
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    /// Method to apply damage to the enemy.
    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// Handles the enemy's death.
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        // Instantiate death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Play death animation or sound here if needed

        // Destroy the enemy game object
        Destroy(gameObject);
    }
}
