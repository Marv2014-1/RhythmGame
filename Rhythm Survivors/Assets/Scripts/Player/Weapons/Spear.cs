using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    [Header("Spear Settings")]
    public GameObject thrustPrefab; // Assign the Thrust prefab in the Inspector
    public Transform thrustSpawnPoint; // Assign the spawn point in the Inspector

    public float thrustSpeed = 10f;
    public float thrustRange = 20f;
    public float rotationSpeed = 5f; // Speed at which the spear rotates
    private Enemy closestEnemy;

    protected override void Start()
    {
        base.Start();

        upgrades = new List<(string, int)>()
        {
            ("Knockback", 5), ("Damage", 5), ("Knockback", 5), ("Pierce", 1)
        };
    }

    void Update()
    {
        UpdateClosestEnemy();
        RotateTowardsClosestEnemy();
    }

    private void UpdateClosestEnemy()
    {
        // Find all enemies within range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, range, enemyLayerMask);

        if (hitEnemies.Length == 0)
        {
            closestEnemy = null;
            return;
        }

        // Identify the closest enemy
        float closestDistance = Mathf.Infinity;
        Enemy nearest = null;

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            float distance = Vector2.Distance(player.transform.position, enemyCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = enemyCollider.GetComponent<Enemy>();
            }
        }

        closestEnemy = nearest;
    }

    private void RotateTowardsClosestEnemy()
    {
        if (closestEnemy == null)
            return;

        Vector2 direction = closestEnemy.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle based on your bow's sprite orientation if needed
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public override void Attack()
    {
        // Instantiate a thrust at the spawn point
        GameObject thrustInstance = Instantiate(thrustPrefab, thrustSpawnPoint.position, transform.rotation, player.transform);

        Thrust thrust = thrustInstance.GetComponent<Thrust>();

        if (thrust != null)
        {
            thrust.SetDamage(damage);
            thrust.SetSpeed(thrustSpeed);
            thrust.SetRange(thrustRange);
        }
        else
        {
            Debug.LogError("Thrust prefab does not have an Thrust component.");
        }
    }
}
