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
    public int cost = 1; // strength of the enemy as seen in the spawner
    public int maxHealth = 100;
    public float baseMoveSpeed = 2f; // Normal move speed
    public float currentMoveSpeed;

    protected int currentHealth;

    [Header("Movement Settings")]
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    private float attackTimer;

    [Header("Death Settings")]
    [SerializeField]
    protected GameObject deathEffect; // Particle effect or animation on death

    [Header("Speed Adjustment Settings")]
    protected float speedBoostMultiplier = 2f;     // Speed increase on beat miss
    protected float speedBoostDuration = 0.2f;        // Duration of speed boost in seconds
    protected float speedSlowdownMultiplier = 1f;  // Speed decrease on beat hit
    protected float speedSlowdownDuration = 0.2f;     // Duration of slowdown in seconds

    protected SpriteRenderer spriteRenderer;

    // Reference to the BeatDetector
    protected BeatDetector beatDetector;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = baseMoveSpeed; // Initialize current speed

        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.AddListener(OnBeatHit);
            beatDetector.OnBeatMissed.AddListener(OnBeatMissed);
        }

        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
    }
    protected virtual void Update()

    {

        // Base enemy update logic

    }



    protected virtual void OnDestroy()
    {
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.RemoveListener(OnBeatHit);
            beatDetector.OnBeatMissed.RemoveListener(OnBeatMissed);
        }
    }

    protected virtual void OnBeatHit()
    {
        StartCoroutine(SlowDown());
    }

    protected virtual void OnBeatMissed()
    {
        StartCoroutine(SpeedUp());
    }

    protected virtual IEnumerator SpeedUp()
    {
        currentMoveSpeed = baseMoveSpeed * speedBoostMultiplier;
        yield return new WaitForSeconds(speedBoostDuration);
        currentMoveSpeed = baseMoveSpeed;
    }

    protected virtual IEnumerator SlowDown()
    {
        currentMoveSpeed = baseMoveSpeed * speedSlowdownMultiplier;
        yield return new WaitForSeconds(speedSlowdownDuration);
        currentMoveSpeed = baseMoveSpeed;
    }

    protected virtual void FixedUpdate()
    {
        MoveTowardsPlayer();
    }

    protected void MoveTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Move only in the x-direction (left and right)
        Vector2 movement = new Vector2(direction.x, 0) * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Flip sprite based on movement direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (FindObjectOfType<ScoreManager>() != null)
        {
            // Find Score Manager and update player's score
            ScoreManager score = FindObjectOfType<ScoreManager>();
            score.UpdateScore(10);
        }

        // Destroy the enemy game object
        Destroy(gameObject);
    }
}
