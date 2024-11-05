using UnityEngine;

public class Arrow : MonoBehaviour
{
	private int damage;
	private float speed;
	private float range;
	private int pierce;
	private float knockback;
	private Vector3 startPosition;

	public void SetDamage(int damageAmount)
	{
		damage = damageAmount;
	}

	public void SetSpeed(float speedAmount)
	{
		speed = speedAmount;
	}

	public void SetRange(float rangeAmount)
	{
		range = rangeAmount;
	}

	public void SetPierce(int pierceAmount)
	{
		pierce = pierceAmount;
	}

	public void SetKnockback(float knockbackAmount)
	{
		knockback = knockbackAmount;
	}

	private void Start()
	{
		startPosition = transform.position;
	}

	private void Update()
	{
		// Move the arrow forward in the direction it's facing
		transform.Translate(Vector3.right * speed * Time.deltaTime);

		// Destroy the arrow after it has traveled the specified range
		if (Vector3.Distance(startPosition, transform.position) >= range)
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Check if the arrow hit an enemy
		if (collision.CompareTag("Enemy"))
		{
			Enemy enemy = collision.GetComponent<Enemy>();

			if (enemy != null)
			{
				// Check if the enemy is invulnerable
				if (!enemy.IsInvulnerable)
				{
					// Knockback the enemy in the opposite direction of the arrow
					Vector2 direction = (enemy.transform.position - transform.position).normalized;
					enemy.Knockback(direction, knockback);
					enemy.TakeDamage(damage);
					Debug.Log($"Arrow hit {enemy.gameObject.name} and dealt {damage} damage.");

					// Decrease pierce count
					if (pierce > 0)
					{
						pierce--;
					}
					else
					{
						Destroy(gameObject);
					}
				}
				else
				{
					// Enemy is invulnerable, do not decrease pierce or destroy the arrow
					Debug.Log($"Arrow hit {enemy.gameObject.name}, but enemy is invulnerable.");
				}
			}
		}
	}
}
