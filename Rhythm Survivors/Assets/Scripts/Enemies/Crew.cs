using UnityEngine;

/// Orc enemy that follows the player and attacks when in range.
public class Crew : Enemy
{
	[Header("Crew Settings")]
	public float attackRange = 1.5f;      // Distance within which Orc can attack
	public float crewAttackCooldown = 2f; // Time between attacks
	public float knockbackForce = 5f;

	private PlayerHealth playerHealth;
	private float lastAttackTime;

	protected override void Awake()
	{
		base.Awake(); // Initialize base class

		// Get the PlayerHealth component
		if (playerTransform != null)
		{
			playerHealth = playerTransform.GetComponent<PlayerHealth>();
			if (playerHealth == null)
			{
				Debug.LogError("PlayerHealth component not found on Player.");
			}
		}

		// Initialize attack timer to allow immediate attack if in range
		lastAttackTime = -crewAttackCooldown;
	}

	/// Handles enemy movement towards the player.
	protected override void FixedUpdate()
	{
		base.FixedUpdate(); // Move towards the player using currentMoveSpeed
	}

	/// Called every frame to check attack conditions.
	protected override void Update()
	{
		if (canMove)
		{
			AttemptAttack();
		}
	}

	/// Attempts to attack the player if conditions are met.
	private void AttemptAttack()
	{
		if (playerTransform == null || playerHealth == null)
			return;

		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

		if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + crewAttackCooldown)
		{
			AttackPlayer();
			lastAttackTime = Time.time;
		}
	}
	/// Attacks the player by dealing damage.
	private void AttackPlayer()
	{
		if (playerHealth != null)
		{
			// Calculate knockback direction: from enemy to player
			Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;

			// Apply damage with knockback
			playerHealth.TakeDamage(attackDamage, knockbackDirection, knockbackForce);
		}
	}

	/// Overrides the Die method to include Orc-specific death behavior.
	protected override void Die()
	{
		base.Die();
		// Add Orc-specific death behavior (e.g., drop loot, play death sound)
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		// Ensure you have tagged the player GameObject with "Player"
		if (other.CompareTag("Player"))
		{
			ApplyDamageToPlayer();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Ensure you have tagged the player GameObject with "Player"
		if (collision.gameObject.CompareTag("Player"))
		{
			ApplyDamageToPlayer();
		}
	}

	private void ApplyDamageToPlayer()
	{
		if (playerHealth != null)
		{
			// Calculate knockback direction: from enemy to player
			Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;

			// Apply damage with knockback
			playerHealth.TakeDamage(attackDamage, knockbackDirection, knockbackForce);
		}
	}
}
