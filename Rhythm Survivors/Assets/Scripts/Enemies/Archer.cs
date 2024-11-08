

using UnityEngine;
using UnityEngine.UI;

public class Archer : Enemy
{
	[Header("Arrow Settings")]
	[SerializeField] private GameObject ArrowPrefab;
	[SerializeField] private Transform ShootPoint;
	public float knockbackForce = 5f;
	private EnemyArrow arrow;
	Vector2 arrowDirection;
	Quaternion rotation;
	float angle;

	[Header("Bow Settings")]
	[Range(0, 10)]
	[SerializeField] public float BowPower = 30f; 

	[Header("Attack Settings")]
	[SerializeField] protected float attackRange = 10f; // Range within which the enemy attacks
	[SerializeField] private float fireRate = 2f;       // Cooldown between shots
	private float StayAwayDistance = 6f;                 //  distance to maintain from the player

	private float nextFireTime = 0f;

	protected override void Update()
	{
		base.Update();

		if (playerTransform == null) return;

		// Calculate distance to the player
		float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

		// Check if player is within attack range and if cooldown has passed
		if (distanceToPlayer <= attackRange)
		{
			if (Time.time >= nextFireTime)
			{
				//stay away at distance from player
				if (distanceToPlayer <= StayAwayDistance)
				{
					canMove = false;
				}

				FireBow();
				nextFireTime = Time.time + fireRate; // Reset cooldown
			}
		}
		else
		{
			canMove = true;
			MoveTowardsPlayer();
		}
	}




	private void FireBow()
	{   // disable moving
		canMove = false;


		// Determine the firing direction directly towards the player
		arrowDirection = (playerTransform.position - ShootPoint.position).normalized;

		// Calculate the angle for the arrow to face
		angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

		// Create a rotation based on the angle
		rotation = Quaternion.Euler(0f, 0f, angle);

		if (animator != null)
		{

			animator.SetTrigger("TriggerAttack1"); // Trigger movement animation
		}
	}
	// shoot arrow
	public void ShootArrow()
	{
		arrow = Instantiate(ArrowPrefab, ShootPoint.position, rotation).GetComponent<EnemyArrow>();
		// shoot arrow if not null
		if (arrow != null)
		{
			arrow.Initialize(arrowDirection, BowPower, attackDamage, attackRange, knockbackForce);
		}
	}
	// Called at the end of the attack animation to reset movement
	public void FinishAttack()
	{	//take max health damage
		// TakeDamage(maxHealth/2); // debugging only
		canMove = true;
	}
}
