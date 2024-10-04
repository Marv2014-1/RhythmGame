using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract base class for all enemies.
/// Handles health, death, movement, and speed adjustments based on beats.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField]
    public int maxHealth = 100;
    [SerializeField]
    public float baseMoveSpeed = 2f; // Normal move speed
    public float currentMoveSpeed;

    protected int currentHealth;

    [Header("Movement Settings")]
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    [Header("Death Settings")]
    [SerializeField]
    protected GameObject deathEffect; // Particle effect or animation on death

    [Header("Speed Adjustment Settings")]
    [SerializeField]
    protected float speedBoostMultiplier = 2f;     // Speed increase on beat miss
    [SerializeField]
    protected float speedBoostDuration = 0.2f;        // Duration of speed boost in seconds

    [SerializeField]
    protected float speedSlowdownMultiplier = 1f;  // Speed decrease on beat hit
    [SerializeField]
    protected float speedSlowdownDuration = 0.2f;     // Duration of slowdown in seconds

    // Reference to the BeatDetector
    protected BeatDetector beatDetector;

    // Initialization
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = baseMoveSpeed; // Initialize current speed

        // Get Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on Enemy.");
        }

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }

        // Find the BeatDetector in the scene and subscribe to events
        beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.AddListener(OnBeatHit);
            beatDetector.OnBeatMissed.AddListener(OnBeatMissed);
        }
        else
        {
            Debug.LogError("BeatDetector not found in the scene.");
        }
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe from the events to prevent memory leaks
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.RemoveListener(OnBeatHit);
            beatDetector.OnBeatMissed.RemoveListener(OnBeatMissed);
        }
    }

    /// <summary>
    /// Called when the player successfully hits a beat.
    /// Slows down the enemy.
    /// </summary>
    protected virtual void OnBeatHit()
    {
        StartCoroutine(SlowDown());
    }

    /// <summary>
    /// Called when the player misses a beat.
    /// Speeds up the enemy.
    /// </summary>
    protected virtual void OnBeatMissed()
    {
        StartCoroutine(SpeedUp());
    }

    /// <summary>
    /// Coroutine to speed up the enemy temporarily.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator SpeedUp()
    {
        currentMoveSpeed = baseMoveSpeed * speedBoostMultiplier;
        yield return new WaitForSeconds(speedBoostDuration);
        currentMoveSpeed = baseMoveSpeed;
    }

    /// <summary>
    /// Coroutine to slow down the enemy temporarily.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator SlowDown()
    {
        currentMoveSpeed = baseMoveSpeed * speedSlowdownMultiplier;
        yield return new WaitForSeconds(speedSlowdownDuration);
        currentMoveSpeed = baseMoveSpeed;
    }

    /// <summary>
    /// Handles enemy movement towards the player.
    /// Called in FixedUpdate for consistent physics handling.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        MoveTowardsPlayer();
    }

    /// <summary>
    /// Moves the enemy towards the player's position.
    /// </summary>
    protected virtual void MoveTowardsPlayer()
    {
        if (playerTransform == null)
            return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector2 movement = direction * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    /// <summary>
    /// Method to apply damage to the enemy.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to apply.</param>
    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the enemy's death.
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        // Instantiate death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy the enemy game object
        Destroy(gameObject);
    }
}
