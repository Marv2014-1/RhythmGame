// using UnityEngine;

// /// <summary>
// /// Orc enemy that follows the player and attacks when in range.
// /// </summary>
// public class Crew : Enemy
// {
// 	[Header("Crew Settings")]
// 	public float attackRange = 1.5f;      // Distance within which Orc can attack
// 	public float crewAttackCooldown = 2f; // Time between attacks
// 	public int damageAmount = 10;         // Damage dealt to the player

// 	private PlayerHealth playerHealth;
// 	private float lastAttackTime;

// 	protected override void Awake()
// 	{
// 		base.Awake(); // Initialize base class

// 		// Get the PlayerHealth component
// 		if (playerTransform != null)
// 		{
// 			playerHealth = playerTransform.GetComponent<PlayerHealth>();
// 			if (playerHealth == null)
// 			{
// 				Debug.LogError("PlayerHealth component not found on Player.");
// 			}
// 		}

// 		// Initialize attack timer to allow immediate attack if in range
// 		lastAttackTime = -crewAttackCooldown;
// 	}

// 	/// <summary>
// 	/// Handles enemy movement towards the player.
// 	/// </summary>
// 	protected override void FixedUpdate()
// 	{
// 		base.FixedUpdate(); // Move towards the player using currentMoveSpeed
// 	}

// 	/// <summary>
// 	/// Called every frame to check attack conditions.
// 	/// </summary>
// 	protected override void Update()
// 	{
// 		AttemptAttack();
// 	}

// 	/// <summary>
// 	/// Attempts to attack the player if conditions are met.
// 	/// </summary>
// 	private void AttemptAttack()
// 	{
// 		if (playerTransform == null || playerHealth == null)
// 			return;

// 		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

// 		if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + crewAttackCooldown)
// 		{
// 			AttackPlayer();
// 			lastAttackTime = Time.time;
// 		}
// 	}

// 	/// <summary>
// 	/// Attacks the player by dealing damage.
// 	/// </summary>
// 	private void AttackPlayer()
// 	{
// 		if (playerHealth != null)
// 		{
// 			playerHealth.TakeDamage(damageAmount);
// 			Debug.Log($"{gameObject.name} attacked the player for {damageAmount} damage.");
// 			// Optionally, trigger attack animations or effects here
// 		}
// 	}

// 	/// <summary>
// 	/// Overrides the Die method to include Orc-specific death behavior.
// 	/// </summary>
// 	protected override void Die()
// 	{
// 		base.Die();
// 		// Add Orc-specific death behavior (e.g., drop loot, play death sound)
// 	}
// }



using UnityEngine;

public class Crew : Enemy
{
    [Header("Crew Settings")]
    public float crewAttackRange = 1.5f;      // Distance within which Crew can attack
    public int damageAmount = 10;             // Damage dealt to the player
    private PlayerHealth playerHealth;

    protected override void Awake()
    {
        base.Awake();

        // Reference to the player's health component
        if (playerTransform != null)
        {
            playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found on Player.");
            }
        }
    }

    protected override void Update()
    {
        // Run the base class's state machine update
        base.Update();

        // Transition to Attack state if within attack range
        if (currentState == EnemyState.Chase && Vector2.Distance(transform.position, playerTransform.position) <= crewAttackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    // Crew-specific melee attack logic
    protected override void Attack()
    {
        if (playerTransform == null || playerHealth == null) return;

        // Check if cooldown allows an attack
        if (Time.time >= attackTimer)
        {
            attackTimer = Time.time + attackCooldown;
            playerHealth.TakeDamage(damageAmount);
            Debug.Log($"{gameObject.name} attacked the player for {damageAmount} damage.");

            // Trigger attack animations if any
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Return to Chase if the player moves out of range
            if (Vector2.Distance(transform.position, playerTransform.position) > crewAttackRange)
            {
                currentState = EnemyState.Chase;
            }
        }
    }

    // Crew-specific death handling
    protected override void Die()
    {
        base.Die();
        Debug.Log($"{gameObject.name} has died and dropped loot.");

        // Optional: Add Crew-specific death effects or loot drop here
    }
}
