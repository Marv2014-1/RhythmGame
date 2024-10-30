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

	private float nextFireTime = 0f;

	protected override void Update()
	{
		base.Update();

		if (playerTransform == null || !canMove) return;

		// Calculate distance to the player
		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

		// Check if player is within attack range
		if (distanceToPlayer <= attackRange)
		{
			// Flip sprite to face the player (left or right)
			Vector2 direction = (playerTransform.position - transform.position).normalized;
			if (spriteRenderer != null)
			{
				spriteRenderer.flipX = direction.x < 0;
			}

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
    int damage = 10; // Adjust as needed
    arrow.Initialize(arrowDirection, BowPower, damage, attackRange);
}

}
