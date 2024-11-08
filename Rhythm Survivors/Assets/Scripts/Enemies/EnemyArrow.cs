using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public int ArrowDamage;
    [SerializeField] Rigidbody2D rb;

    private Vector2 startPosition;
    private float maxTravelDistance;

    private float knockback;

    private void Start()
	{
		startPosition = transform.position;
	}
    public void Initialize(Vector2 direction, float speed, int damage, float attackRange, float knockback)
    {
        this.speed = speed;
        ArrowDamage = damage;
        rb.velocity = direction * speed;
        startPosition = transform.position;
        maxTravelDistance = attackRange * 2f;
        this.knockback = knockback;
    }

    private void Update()
    {
         
        // Check the distance traveled
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        float distanceTraveled = Vector2.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            try
            {
                // Assume the player has a method to take damage and apply knockback
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(ArrowDamage, direction, knockback);
                Destroy(gameObject); // Destroy the arrow upon hitting the player
            }
            catch (System.Exception e)
            {
                Debug.Log("PlayerHealth component not found on Player." + e.Message);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Ignore other enemies
            return;
        }
    }
}
