// using UnityEngine;
// using System.Collections;

// /// <summary>
// /// Abstract base class for all enemies.
// /// Handles health, death, movement, and speed adjustments based on beats.
// /// </summary>
// [RequireComponent(typeof(Collider2D))]
// [RequireComponent(typeof(Rigidbody2D))]
// public abstract class Enemy : MonoBehaviour
// {
//     [Header("Enemy Stats")]

//     public int cost = 1; // strength of the enemy as seen in the spawner
//     public int maxHealth = 100;
//     public float baseMoveSpeed = 2f; // Normal move speed
//     public float currentMoveSpeed;

//     protected int currentHealth;

//     [Header("Movement Settings")]
//     protected Rigidbody2D rb;
//     protected Transform playerTransform;

//     [Header("Attack")]
//     public float attackDamage = 10f;
//     public float attackCooldown = 1f;
//     private float attackTimer;

//     [Header("Death Settings")]
//     [SerializeField]
//     protected GameObject deathEffect; // Particle effect or animation on death

//     [Header("Speed Adjustment Settings")]
//     protected float speedBoostMultiplier = 2f;     // Speed increase on beat miss
//     protected float speedBoostDuration = 0.2f;        // Duration of speed boost in seconds
//     protected float speedSlowdownMultiplier = 1f;  // Speed decrease on beat hit
//     protected float speedSlowdownDuration = 0.2f;     // Duration of slowdown in seconds

//     protected SpriteRenderer spriteRenderer;
//     protected Animator animator;

//     // Reference to the BeatDetector
//     protected BeatDetector beatDetector;

//     protected virtual void Awake()
//     {
//         currentHealth = maxHealth;
//         currentMoveSpeed = baseMoveSpeed; // Initialize current speed

//         rb = GetComponent<Rigidbody2D>();
//         playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

//         beatDetector = FindObjectOfType<BeatDetector>();
//         if (beatDetector != null)
//         {
//             beatDetector.OnBeatHit.AddListener(OnBeatHit);
//             beatDetector.OnBeatMissed.AddListener(OnBeatMissed);
//         }

//         spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
//         animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
//     }

//     protected virtual void Update()

//     {

//         // Base enemy update logic

//     }



//     protected virtual void OnDestroy()
//     {
//         if (beatDetector != null)
//         {
//             beatDetector.OnBeatHit.RemoveListener(OnBeatHit);
//             beatDetector.OnBeatMissed.RemoveListener(OnBeatMissed);
//         }
//     }

//     protected virtual void OnBeatHit()
//     {
//         StartCoroutine(SlowDown());
//     }

//     protected virtual void OnBeatMissed()
//     {
//         StartCoroutine(SpeedUp());
//     }

//     protected virtual IEnumerator SpeedUp()
//     {
//         currentMoveSpeed = baseMoveSpeed * speedBoostMultiplier;
//         yield return new WaitForSeconds(speedBoostDuration);
//         currentMoveSpeed = baseMoveSpeed;
//     }

//     protected virtual IEnumerator SlowDown()
//     {
//         currentMoveSpeed = baseMoveSpeed * speedSlowdownMultiplier;
//         yield return new WaitForSeconds(speedSlowdownDuration);
//         currentMoveSpeed = baseMoveSpeed;
//     }

//     protected virtual void FixedUpdate()
//     {
//         MoveTowardsPlayer();
//     }

//     protected void MoveTowardsPlayer()
//     {
//         if (playerTransform == null) return;

//         Vector2 direction = (playerTransform.position - transform.position).normalized;

//         animator.SetBool("IsMoving", true);

//         // Move in both x and y directions
//         Vector2 movement = direction * currentMoveSpeed * Time.fixedDeltaTime;
//         rb.MovePosition(rb.position + movement);

//         // Optional: Flip sprite based on horizontal movement
//         if (spriteRenderer != null)
//         {
//             spriteRenderer.flipX = direction.x < 0;
//         }
//     }

//     public virtual void TakeDamage(int damageAmount)
//     {
//         currentHealth -= damageAmount;
//         Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}");

//         animator.SetBool("IsHurt", true);

//         if (currentHealth <= 0)
//         {
//             Die();
//         }
//     }

//     protected virtual void Die()
//     {
//         Debug.Log($"{gameObject.name} has died.");
//         if (FindObjectOfType<ScoreManager>() != null)
//         {
//             // Find Score Manager and update player's score
//             ScoreManager score = FindObjectOfType<ScoreManager>();
//             score.UpdateScore(cost*10);
//         }

//         animator.SetBool("IsDead", true);

//         if (deathEffect != null)
//         {
//             Instantiate(deathEffect, transform.position, Quaternion.identity);
//         }
//     }
// }







// using UnityEngine;
// using System.Collections;
// [RequireComponent(typeof(Collider2D))]
// [RequireComponent(typeof(Rigidbody2D))]
// public abstract class Enemy : MonoBehaviour
// {
//     public enum EnemyState { Idle, Chase, Attack, Die }
//     public EnemyState currentState = EnemyState.Idle;

//     [Header("Enemy Settings")]
//     public int cost = 1; // strength of the enemy as seen in the spawner
//     public int maxHealth = 100;
//     public float baseMoveSpeed = 2f;
//     public float detectRange = 7f;
//     public float attackRange = 2f;
//     public float attackCooldown = 1f;
//     protected SpriteRenderer spriteRenderer;

//     [Header("Death Settings")]
//     [SerializeField]
//     protected GameObject deathEffect; // Particle effect or animation on death

//     [Header("Obstacle Avoidance")]
//     public LayerMask obstacleLayerMask;
//     public float obstacleAvoidanceDistance = 1f;

//     protected Transform playerTransform;
//     protected Rigidbody2D rb;
//     protected Animator animator;
//     protected float currentMoveSpeed;
//     protected float attackTimer;
//     private int currentHealth;

//     protected virtual void Awake()
//     {
//         currentHealth = maxHealth;
//         currentMoveSpeed = baseMoveSpeed;
//         rb = GetComponent<Rigidbody2D>();
//         playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
//         animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
//     }

//     protected virtual void Update()
//     {
//         switch (currentState)
//         {
//             case EnemyState.Idle:
//                 Idle();
//                 break;
//             case EnemyState.Chase:
//                 ChasePlayer();
//                 break;
//             case EnemyState.Attack:
//                 if (Time.time >= attackTimer)
//                 {
//                     Attack();
//                     attackTimer = Time.time + attackCooldown;
//                 }
//                 break;
//             case EnemyState.Die:
//                 Die();
//                 break;
//         }
//     }

//     protected virtual void Idle()
//     {
//         // Transition to Chase state if player is within detectRange
//         if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) < detectRange)
//         {
//             currentState = EnemyState.Chase;
//         }
//     }

//     protected virtual void ChasePlayer()
//     {
//         if (playerTransform == null) return;

//         // Move towards the player while checking for obstacles
//         MoveTowards(playerTransform.position);

//         // Check if the enemy is close enough to attack
//         if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
//         {
//             currentState = EnemyState.Attack;
//         }
//         else if (currentHealth <= 0)
//         {
//             currentState = EnemyState.Die;
//         }
//     }

//     protected virtual void Attack()
//     {
//         // Implement specific attack logic in subclasses
//     }

//     protected virtual void Die()
//     {
//         Debug.Log($"{gameObject.name} has died.");

//         // Update score if ScoreManager is found
//         ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
//         if (scoreManager != null)
//         {
//             scoreManager.UpdateScore(cost * 10);
//         }

//         // Play death animation, if any
//         if (animator != null) animator.SetBool("IsDead", true);

//         // Instantiate death effect (e.g., particles), if assigned
//         if (deathEffect != null)
//         {
//             Instantiate(deathEffect, transform.position, Quaternion.identity);
//         }

//         // Destroy the enemy game object
//         Destroy(gameObject);
//     }

//     // Move towards a target position with obstacle avoidance and animation handling
//     protected void MoveTowards(Vector2 targetPosition)
//     {
//         if (playerTransform == null) return;

//         // Calculate movement direction
//         Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

//         // Obstacle detection with raycast
//         RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleAvoidanceDistance, obstacleLayerMask);
//         if (hit.collider != null)
//         {
//             // If an obstacle is detected, move perpendicular to the obstacle's surface
//             Vector2 avoidDirection = Vector2.Perpendicular(hit.normal) * currentMoveSpeed;
//             rb.MovePosition(rb.position + avoidDirection * Time.fixedDeltaTime);
//         }
//         else
//         {
//             // Move directly towards the target if no obstacle is detected
//             Vector2 movement = direction * currentMoveSpeed * Time.fixedDeltaTime;
//             rb.MovePosition(rb.position + movement);
//         }

//         // Set moving animation
//         if (animator != null) animator.SetBool("IsMoving", true);

//         // Flip the sprite based on movement direction
//         if (spriteRenderer != null)
//         {
//             spriteRenderer.flipX = direction.x < 0;
//         }
//     }

//     // Handle enemy damage and transition to Die state if health reaches zero
//     public virtual void TakeDamage(int damageAmount)
//     {
//         currentHealth -= damageAmount;
//         if (currentHealth <= 0)
//         {
//             currentState = EnemyState.Die;
//         }
//     }
// }

























using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Die }
    public EnemyState currentState = EnemyState.Idle;

    [Header("Enemy Settings")]
    public int cost = 1;
    public int maxHealth = 100;
    public float baseMoveSpeed = 2f;
    public float detectRange = 7f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    protected SpriteRenderer spriteRenderer;

    [Header("Death Settings")]
    [SerializeField] protected GameObject deathEffect;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayerMask;
    public float obstacleAvoidanceDistance = 1f;

    [Header("Beat Adjustment Settings")]
    public float speedBoostMultiplier = 2f;
    public float speedBoostDuration = 0.2f;
    public float speedSlowdownMultiplier = 0.5f;
    public float speedSlowdownDuration = 0.2f;

    private BeatDetector beatDetector;
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected float currentMoveSpeed;
    protected float attackTimer;
    private int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = baseMoveSpeed;
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();

        // Initialize and add listeners for BeatDetector
        beatDetector = FindObjectOfType<BeatDetector>();
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.AddListener(OnBeatHit);
            beatDetector.OnBeatMissed.AddListener(OnBeatMissed);
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

    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                if (Time.time >= attackTimer)
                {
                    Attack();
                    attackTimer = Time.time + attackCooldown;
                }
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    protected virtual void Idle()
    {
        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) < detectRange)
        {
            currentState = EnemyState.Chase;
        }
    }

    protected virtual void ChasePlayer()
    {
        if (playerTransform == null) return;

        MoveTowards(playerTransform.position);

        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (currentHealth <= 0)
        {
            currentState = EnemyState.Die;
        }
    }

    protected virtual void Attack()
    {
        // Implement specific attack logic in subclasses
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.UpdateScore(cost * 10);
        }

        if (animator != null) animator.SetBool("IsDead", true);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // Move towards a target position with obstacle avoidance
    protected void MoveTowards(Vector2 targetPosition)
    {
        if (playerTransform == null) return;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleAvoidanceDistance, obstacleLayerMask);

        if (hit.collider != null)
        {
            Vector2 avoidDirection = Vector2.Perpendicular(hit.normal) * currentMoveSpeed;
            rb.MovePosition(rb.position + avoidDirection * Time.fixedDeltaTime);
        }
        else
        {
            Vector2 movement = direction * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }

        if (animator != null) animator.SetBool("IsMoving", true);
        if (spriteRenderer != null) spriteRenderer.flipX = direction.x < 0;
    }

    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentState = EnemyState.Die;
        }
    }

    // Beat event: Speed up
    protected virtual void OnBeatHit()
    {
        StartCoroutine(SpeedUp());
    }

    // Beat event: Slow down
    protected virtual void OnBeatMissed()
    {
        StartCoroutine(SlowDown());
    }

    // Coroutine to temporarily increase speed
    protected IEnumerator SpeedUp()
    {
        currentMoveSpeed = baseMoveSpeed * speedBoostMultiplier;
        yield return new WaitForSeconds(speedBoostDuration);
        currentMoveSpeed = baseMoveSpeed;
    }

    // Coroutine to temporarily decrease speed
    protected IEnumerator SlowDown()
    {
        currentMoveSpeed = baseMoveSpeed * speedSlowdownMultiplier;
        yield return new WaitForSeconds(speedSlowdownDuration);
        currentMoveSpeed = baseMoveSpeed;
    }
}



