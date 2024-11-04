using UnityEngine;
using UnityEngine.UI;

public class Rogue : Enemy
{
	[Header("Arrow Settings")]
	[SerializeField] private GameObject ArrowPrefab;
	[SerializeField] private Transform ShootPoint;

	[Header("Bow Settings")]
	[Range(0, 10)]
	[SerializeField] public float BowPower = 5f;

	[Header("Attack Settings")]
	[SerializeField] private float attackRange = 10f; // Range within which the enemy attacks
	[SerializeField] private float fireRate = 2f;     // Cooldown between shots

	private int damage = 10;
	public float knockbackForce = 5f;

	private float nextFireTime = 0f;

	private PlayerHealth playerHealth;

	protected override void Awake()
	{
		base.Awake();

		// Get the PlayerHealth component
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
		base.Update();

		if (playerTransform == null || !canMove) return;

		// Calculate distance to the player
		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

		// Check if player is within attack range
		if (distanceToPlayer <= attackRange)
		{
			// Handle firing based on cooldown
			if (Time.time >= nextFireTime)
			{
				FireBow();
				nextFireTime = Time.time + fireRate; // Reset cooldown
			}
		}
	}

	private void FireBow()
	{
		// Determine the firing direction directly towards the player
		Vector2 arrowDirection = (playerTransform.position - ShootPoint.position).normalized;

		// Calculate the angle for the arrow to face
		float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

		// Create a rotation based on the angle
		Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

		// Instantiate and initialize the arrow with the calculated rotation
		var arrow = Instantiate(ArrowPrefab, ShootPoint.position, rotation).GetComponent<EnemyArrow>();
		arrow.Initialize(arrowDirection, BowPower, damage, attackRange, knockbackForce);
	}

	// player takes damage on collision with enemy
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

	// Apply damage to the player and knock them back
	private void ApplyDamageToPlayer()
	{
		if (playerHealth != null)
		{
			// Calculate knockback direction: from enemy to player
			Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;

			// Apply damage with knockback
			playerHealth.TakeDamage(damage, knockbackDirection, knockbackForce);
		}
	}


}
