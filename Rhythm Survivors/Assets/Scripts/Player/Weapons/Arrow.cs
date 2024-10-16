using UnityEngine;

public class Arrow : MonoBehaviour
{
	private int damage;
	private float speed;
	private float range;
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
				enemy.TakeDamage(damage);
				Debug.Log($"Arrow hit {enemy.gameObject.name} and dealt {damage} damage.");
			}

			// Destroy the arrow
			Destroy(gameObject);
		}
	}
}
