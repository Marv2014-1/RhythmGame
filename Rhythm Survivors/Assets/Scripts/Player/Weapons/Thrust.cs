using UnityEngine;

public class Thrust : MonoBehaviour
{
    private int damage;
    private float speed;
    private float range;
    private float knockback;
    private float size;
    private Vector3 startPosition;
    private Quaternion startRotation;

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

    public void SetKnockback(float knockbackAmount)
    {
        knockback = knockbackAmount;
    }

    public void SetSize(float sizeAmount)
    {
        size = sizeAmount;
    }

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        transform.localScale = new Vector3(size / 10, size / 10, 1);
    }

    private void Update()
    {
        // Prevent thrust from exiting specified range
        if (Vector3.Distance(startPosition, transform.position) < range)
        {
            // Move the spear forward in the direction it's facing
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        transform.rotation = startRotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the arrow hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                enemy.Knockback(direction, knockback);
                enemy.TakeDamage(damage);
                Debug.Log($"Spear hit {enemy.gameObject.name} and dealt {damage} damage.");
            }
        }
    }
}
