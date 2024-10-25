using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab; // Assign the Arrow prefab in the Inspector
    public Transform arrowSpawnPoint; // Assign the spawn point in the Inspector

    public float arrowSpeed = 10f;
    public float arrowRange = 20f;
    public float rotationSpeed = 5f; // Speed at which the bow rotates

    private Enemy closestEnemy;

    protected override void Start()
    {
        base.Start();
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

    protected override void OnBeatDetected()
    {
        beatCount++;
        UpdateWeaponUI(beatCount);

        if (beatCount >= requiredBeats)
        {
            Attack();
            beatCount = 0; // Reset beat count after attack
            UpdateWeaponUI(beatCount);
        }
    }

    public override void Attack()
    {
        // Instantiate an arrow at the spawn point with the same rotation as the bow
        GameObject arrowInstance = Instantiate(arrowPrefab, arrowSpawnPoint.position, transform.rotation);

        Arrow arrow = arrowInstance.GetComponent<Arrow>();

        if (arrow != null)
        {
            arrow.SetDamage(damage);
            arrow.SetSpeed(arrowSpeed);
            arrow.SetRange(arrowRange);

            Debug.Log("Bow shot an arrow.");
        }
        else
        {
            Debug.LogError("Arrow prefab does not have an Arrow component.");
        }
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
