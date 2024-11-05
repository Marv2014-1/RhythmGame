using UnityEngine;
using UnityEngine.UI;

public class Archer : Enemy
{
	[Header("Arrow Settings")]
	[SerializeField] private GameObject ArrowPrefab;
	[SerializeField] private Transform ShootPoint;
	public float knockbackForce = 5f;

	[Header("Bow Settings")]
	[Range(0, 10)]
	[SerializeField] public float BowPower = 5f;

	[Header("Attack Settings")]
	[SerializeField] protected float attackRange = 10f; // Range within which the enemy attacks
	[SerializeField] private float fireRate = 2f;       // Cooldown between shots
	private float minimumDistance = 10f;                 // Minimum distance to maintain from the player

	private float nextFireTime = 0f;

	protected override void Update()
	{
		base.Update();

		if (playerTransform == null) return;

		// Calculate distance to the player
		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

		// Move towards the player if further than the minimum distance
		// if (distanceToPlayer > minimumDistance)
		// {
		//     MoveTowardsPlayer();
		// }

		// Check if player is within attack range and if cooldown has passed
		if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
		{
			FireBow();
			//enable moving
			movable = true;
			nextFireTime = Time.time + fireRate; // Reset cooldown
		}
	}


	// private void MoveTowardsPlayer()
	// {
	//     Vector2 direction = (playerTransform.position - transform.position).normalized;
	//     Vector2 newPosition = Vector2.MoveTowards(transform.position, playerTransform.position, currentMoveSpeed * Time.deltaTime);

	//     // Update position to approach the player but not get closer than minimumDistance
	//     transform.position = newPosition;

	//     // Flip sprite to face the player
	//     if (spriteRenderer != null)
	//     {
	//         spriteRenderer.flipX = direction.x < 0;
	//     }
	// }

	private void FireBow()
	{	// disable moving
	    movable = false;
		if (animator != null)
		{
			animator.SetBool("IsAttack", true); // Trigger movement animation
		}
		// Determine the firing direction directly towards the player
		Vector2 arrowDirection = (playerTransform.position - ShootPoint.position).normalized;

		// Calculate the angle for the arrow to face
		float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

		// Create a rotation based on the angle
		Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

		// Instantiate and initialize the arrow with the calculated rotation
		var arrow = Instantiate(ArrowPrefab, ShootPoint.position, rotation).GetComponent<EnemyArrow>();
		int damage = 10; // Adjust as needed
		arrow.Initialize(arrowDirection, BowPower, damage, attackRange, knockbackForce);
	}
}
