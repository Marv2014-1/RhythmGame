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
    public float detectionRadius = 5f; // Adjust as needed for each enemy type
    public int cost = 1; // strength of the enemy as seen in the spawner
    public int maxHealth = 100;
    public float baseMoveSpeed = 2f; // Normal move speed
    public float currentMoveSpeed;
    protected bool movable = true;

    protected int currentHealth;

    [Header("Movement Settings")]
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    private readonly float attackTimer;

    [Header("Death Settings")]
    [SerializeField]
    protected GameObject deathEffect; // Particle effect or animation on death

    [Header("Speed Adjustment Settings")]
    protected float speedBoostMultiplier = 2f;     // Speed increase on beat miss
    protected float speedBoostDuration = 0.2f;        // Duration of speed boost in seconds
    protected float speedSlowdownMultiplier = 1f;  // Speed decrease on beat hit
    protected float speedSlowdownDuration = 0.2f;     // Duration of slowdown in seconds

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    // Reference to the BeatDetector
    protected BeatDetector beatDetector;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = baseMoveSpeed; // Initialize current speed

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze Z rotation
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            beatDetector.OnBeatMissed.AddListener(OnBeatMissed);
        }

        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    protected virtual void Update()

    {

        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool isMoving = distanceToPlayer > 0.1f; // Consider moving if distance is significant

        // Update "IsMoving" based on whether the enemy is moving
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }


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
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector2 movement = currentMoveSpeed * Time.fixedDeltaTime * direction;
        rb.MovePosition(rb.position + movement);
    }

    protected void MoveTowardsPlayer()
    {
        if (movable == true)
        {
            if (playerTransform == null) return;

            Vector2 direction = (playerTransform.position - transform.position).normalized;

            animator.SetBool("IsMoving", true);

            // Move in both x and y directions
            Vector2 movement = direction * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
            // Optional: Flip sprite based on horizontal movement
            if (spriteRenderer != null)
            {
                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Face right
                }
                else if (direction.x < 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Face left
                }

            }

            // if (spriteRenderer != null)
            // {
            //     if (direction.x >= 0)
            //     {
            //         transform.localScale = new Vector3(10, 10, 1);
            //     }
            //     else
            //     {
            //         transform.localScale = new Vector3(-10, 10, 1);
            //     }
            // }
            if (animator != null)
            {
                animator.SetBool("IsMoving", true); // Trigger movement animation
            }
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}");

        animator.SetBool("IsHurt", true);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        if (FindObjectOfType<ScoreManager>() != null)
        {
            // Find Score Manager and update player's score
            ScoreManager score = FindObjectOfType<ScoreManager>();
            score.UpdateScore(cost * 10);
        }

        animator.SetBool("IsDead", true);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

    }
}