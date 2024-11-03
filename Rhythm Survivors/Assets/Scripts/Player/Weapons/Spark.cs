using System.Collections;
using UnityEngine;

public class Spark : MonoBehaviour
{
    private int damage;
    private float speed;
    private float range;
    private Vector3 startPosition;
    private Vector2 direction;

    public ParticleSystem trailParticles; // Assign a particle system for the trail in the Inspector
    public float rotationSpeed = 360f;    // Speed of spinning in degrees per second
    public float destroyDelay = 5f;       // Time to wait before destroying the GameObject

    private SpriteRenderer spriteRenderer;
    private Collider2D boxCollider;

    private void Awake()
    {
        // Cache references to components
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<Collider2D>();
    }

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

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // Ensure direction is normalized
    }

    private void Start()
    {
        startPosition = transform.position;

        // Start the particle system
        if (trailParticles != null)
        {
            trailParticles.Play();
        }
    }

    private void Update()
    {
        // Move the spark in the specified direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Spin the spark
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Destroy the spark after it has traveled the specified range
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            trailParticles.Clear();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the spark hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"Spark hit {enemy.gameObject.name} and dealt {damage} damage.");
            }

            // Destroy the spark upon hitting an enemy
            trailParticles.Clear();
            Destroy(gameObject);
        }
    }
}
