using UnityEngine;
using UnityEngine.UI;

public class Rogue : Enemy
{
	[Header("Arrow Settings")]
	[SerializeField] private GameObject ArrowPrefab;
	[SerializeField] private Transform Bow;

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

		if (playerTransform == null) return;

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

			// Adjust the bow's aim after flipping
			AdjustBowAim();

			// Handle firing based on cooldown
			if (Time.time >= nextFireTime)
			{
				FireBow();
				nextFireTime = Time.time + fireRate; // Reset cooldown
			}
		}
	}


	private void AdjustBowAim()
	{
		if (Bow == null || playerTransform == null) return;

		// Determine if facing left
		bool isFacingLeft = spriteRenderer != null && spriteRenderer.flipX;

		// Calculate the angle to the player
		Vector2 bowToPlayer = playerTransform.position - Bow.position;
		float angleToPlayer = Mathf.Atan2(bowToPlayer.y, bowToPlayer.x) * Mathf.Rad2Deg;

		// Facing angle is 0 if facing right, 180 if facing left
		float facingAngle = isFacingLeft ? 180f : 0f;

		// Calculate the angle difference between facing angle and angle to player
		float angleDifference = Mathf.DeltaAngle(facingAngle, angleToPlayer);

		// Clamp the angle difference between -45 and 45 degrees
		float clampedAngleDifference = Mathf.Clamp(angleDifference, -45f, 45f);

		// Compute the final angle
		float finalAngle = facingAngle + clampedAngleDifference;

		// Apply rotation to the Bow
		Bow.rotation = Quaternion.Euler(0f, 0f, finalAngle);

		// Flip the bow's local scale when the rogue flips
		Vector3 bowScale = Bow.localScale;
		bowScale.y = isFacingLeft ? -Mathf.Abs(bowScale.y) : Mathf.Abs(bowScale.y);
		Bow.localScale = bowScale;

		// Adjust the bow's local position along the x-axis when the rogue flips
		Vector3 bowPosition = Bow.localPosition;
		bowPosition.x = isFacingLeft ? -Mathf.Abs(bowPosition.x) : Mathf.Abs(bowPosition.x);
		Bow.localPosition = bowPosition;
	}



	private void FireBow()
{
    // Determine the firing direction directly towards the player
    Vector2 arrowDirection = (playerTransform.position - Bow.position).normalized;

    // Calculate the angle for the arrow to face
    float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

    // Create a rotation based on the angle
    Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

    // Instantiate and initialize the arrow with the calculated rotation
    var arrow = Instantiate(ArrowPrefab, Bow.position, rotation).GetComponent<EnemyArrow>();
    int damage = 10; // Adjust as needed
    arrow.Initialize(arrowDirection, BowPower, damage, attackRange);
}

}
