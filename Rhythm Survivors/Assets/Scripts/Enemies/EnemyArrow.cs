using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    [HideInInspector] public float ArrowVelocity;
    [HideInInspector] public int ArrowDamage;
    [SerializeField] Rigidbody2D rb;

    private Vector2 startPosition;
    private float maxTravelDistance;

    private float knockback;

    public void Initialize(Vector2 direction, float speed, int damage, float attackRange, float knockback)
    {
        ArrowVelocity = speed;
        ArrowDamage = damage;
        rb.velocity = direction * ArrowVelocity;
        startPosition = transform.position;
        maxTravelDistance = attackRange * 2f;
        this.knockback = knockback;
    }

    private void Update()
    {

        // Check the distance traveled
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
            // Assume the player has a method to take damage and apply knockback
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(ArrowDamage, direction, knockback);
            Destroy(gameObject); // Destroy the arrow upon hitting the player
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Ignore other enemies
            return;
        }
    }
}
