using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab; // Assign the Arrow prefab in the Inspector

    public float rotationSpeed = 5f; // Speed at which the bow rotates

    private Enemy closestEnemy;

    protected override void Start()
    {
        base.Start();

        upgrades = new List<(string, int)>()
        {
            ("Pierce", 1), ("Range", 5), ("Pierce", 1), ("Damage", 10), ("Range", 5), ("Damage", 5), ("Pierce", 2)
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
        // Instantiate an arrow with the same rotation as the bow
        GameObject arrowInstance = Instantiate(arrowPrefab, this.transform);

        Arrow arrow = arrowInstance.GetComponent<Arrow>();

        if (arrow != null)
        {
            arrow.SetDamage(damage);
            arrow.SetSpeed(speed);
            arrow.SetRange(range);
            arrow.SetPierce(pierce);
            arrow.SetKnockback(knockback);

            Debug.Log("Bow shot an arrow.");
        }
        else
        {
            Debug.LogError("Arrow prefab does not have an Arrow component.");
        }

        arrowInstance.transform.SetParent(null, true);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the range in the editor
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, range);
        }
    }
}
