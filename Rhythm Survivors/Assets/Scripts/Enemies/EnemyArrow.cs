using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    [HideInInspector] public float ArrowVelocity;
    [HideInInspector] public int ArrowDamage;
    [SerializeField] Rigidbody2D rb;

    private Vector2 startPosition;
    private float maxTravelDistance;

    public void Initialize(Vector2 direction, float speed, int damage, float attackRange)
    {
        ArrowVelocity = speed;
        ArrowDamage = damage;
        rb.velocity = direction * ArrowVelocity;
        startPosition = transform.position;
        maxTravelDistance = attackRange * 2f;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Assume the player has a method to take damage
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(ArrowDamage);
            Destroy(gameObject); // Destroy the arrow upon hitting the player
        }
        else
        {
            // Optionally destroy the arrow upon colliding with any object
            Destroy(gameObject);
        }
    }
}
