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
         // transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.forward);
        // transform.Translate(rb.velocity * Time.deltaTime);
    // rotation towards the direction of the arrow follow the velocity
    
    // transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.forward); // This line works perfectly for 2D arrows but not for 3D arrows. For 3D arrows, you might need to use a different approach.

    // if (rb.velocity != Vector2.zero) // Check to avoid errors if velocity is zero
    // {
    //     float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
    //     transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    // }
       


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
            try
            {
                // Assume the player has a method to take damage and apply knockback
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(ArrowDamage, direction, knockback);
                Destroy(gameObject); // Destroy the arrow upon hitting the player
            }
            catch (System.Exception e)
            {
                // Debug.LogError("PlayerHealth component not found on Player.");
                // Debug.LogError(e.ToString());
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Ignore other enemies
            return;
        }
    }
}
