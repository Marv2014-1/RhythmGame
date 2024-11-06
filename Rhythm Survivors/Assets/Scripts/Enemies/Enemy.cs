using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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
    public int xpDrop = 10;
    public float baseMoveSpeed = 2f; // Normal move speed
    public float currentMoveSpeed;
    protected int currentHealth;
    private Shield shield;

    [Header("Movement Settings")]
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    // private NavMeshAgent agent;
    public bool canMove;



    [Header("Attack")]
    public int attackDamage = 10;
    public float attackCooldown = 1f;
    private float attackTimer;

    [Header("Death Settings")]
    [SerializeField]
    protected GameObject deathEffect; // Particle effect or animation on death
    protected PlayerExperience playerXP;

    [Header("Speed Adjustment Settings")]
    protected float speedBoostMultiplier = 2f;     // Speed increase on beat miss
    protected float speedBoostDuration = 0.2f;        // Duration of speed boost in seconds
    protected float speedSlowdownMultiplier = 1f;  // Speed decrease on beat hit
    protected float speedSlowdownDuration = 0.2f;     // Duration of slowdown in seconds

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    private bool isKnockedBack = false;
    public float knockbackDuration = 0.2f;
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 0.2f;

    // Reference to the BeatDetector
    protected BeatDetector beatDetector;

    protected virtual void Awake()

    {
        shield = GetComponent<Shield>();
        currentHealth = maxHealth;
        currentMoveSpeed = baseMoveSpeed; // Initialize current speed
        canMove = true;

        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerXP = FindObjectOfType<PlayerExperience>();

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

        // Set rigidbody properties
        rb.mass = 1f;
        rb.drag = 1f;
        rb.angularDrag = 0.5f;

        // agent = GetComponent<NavMeshAgent>();
        // if (agent == null)
        // {
        //     agent = gameObject.AddComponent<NavMeshAgent>();
        // }
        // // Set up avoidance to avoid both Default and Enemy layers
        // agent.avoidancePriority = Random.Range(10, 50);  // Random priority to reduce collision clustering
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        // // Set NavMeshAgent properties to make sure it avoids other enemies and obstacles
        // agent.areaMask = (1 << NavMesh.GetAreaFromName("Default")) | (1 << NavMesh.GetAreaFromName("Enemy"));

    }

    protected virtual void Update()

    {
        // agent.areaMask = (1 << NavMesh.GetAreaFromName("Default")) | (1 << NavMesh.GetAreaFromName("Enemy"));

        // if (playerTransform == null) return;

        // float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        // bool isMoving = distanceToPlayer > 0.1f; // Consider moving if distance is significant

        // // Update "IsMoving" based on whether the enemy is moving 
        // if (animator != null)
        // {
        //     animator.SetBool("IsMoving", isMoving);
        // }


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
        if (canMove)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected void MoveTowardsPlayer()
    {
        if (!canMove)
        {
            return;
        }
        if (playerTransform == null || isKnockedBack) return;



        Vector2 direction = (playerTransform.position - transform.position).normalized;

        animator.SetBool("IsMoving", true);

        // Move in both x and y directions
        Vector2 movement = direction * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // if (agent.isOnNavMesh)
        // {
        //     agent.SetDestination(playerTransform.position);
        // }
        // else
        // {
        //     Debug.LogWarning("Agent is not on a NavMesh. Ensure NavMesh is baked and agent is placed on it.");
        // }

        if (spriteRenderer != null)
        {
            // Vector3 scale = transform.localScale;
            // scale.x = (agent.desiredVelocity.x >= 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            // transform.localScale = scale;
            if (direction.x >= 0)
            {
                transform.localScale = new Vector3(10, 10, 1);
            }
            else
            {
                transform.localScale = new Vector3(-10, 10, 1);
            }
        }
    }

    // Reduce health when hit
    public virtual void TakeDamage(int damageAmount)
    {
        if (isInvulnerable)
        {

            return;
        } // If invulnerable, ignore damage
        if (shield!= null && shield.IsBlocking())
        {
            animator.SetTrigger("TriggerShield");
            damageAmount = shield.CalculateDamageAfterBlock(damageAmount); // Reduce damage by shield
        }
        // If damage is reduced to 0 by the shield, no further processing is needed
        if (damageAmount <= 0)
        {
            Debug.Log("Damage fully blocked by shield.");
            return;
        }
        canMove = false;
        isInvulnerable = true; // Set invulnerability
        StartCoroutine(InvulnerabilityCooldown()); // Start cooldown

        currentHealth -= damageAmount;

        animator.SetBool("IsHurt", true);

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public bool IsInvulnerable
    {
        get { return isInvulnerable; }
    }

    // Add invulnerability cooldown
    private IEnumerator InvulnerabilityCooldown()
    {
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false; // Remove invulnerability
        canMove = true;
    }

    // Knockback the enemy
    public virtual void Knockback(Vector2 direction, float force)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            StartCoroutine(KnockbackCooldown());
        }
    }

    private IEnumerator KnockbackCooldown()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    // Kill the enemy
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        canMove = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        if (FindObjectOfType<ScoreManager>() != null)
        {
            // Find Score Manager and update player's score
            ScoreManager score = FindObjectOfType<ScoreManager>();
            score.UpdateScore(cost * 10);
        }

        animator.SetBool("IsDead", true);

        playerXP.GetExperience(xpDrop);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

    }
}