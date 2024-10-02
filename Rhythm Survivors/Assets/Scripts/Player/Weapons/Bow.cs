using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    private void Start()
    {
        // Initialize weapon properties
        weaponName = "Bow";
        damage = 10;
        attackSpeed = 1.0f;
        range = 5.0f;
        requiredBeats = 3;

        // Assign the player object
        player = GameObject.FindGameObjectWithTag("Player");

        // Subscribe to the OnBeatHit event
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.AddListener(OnBeatDetected);
        }
        else
        {
            Debug.LogError("BeatDetector reference not assigned in Bow class.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.RemoveListener(OnBeatDetected);
        }
    }

    // This method is called when a beat is detected
    private void OnBeatDetected()
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
        // [Existing attack logic remains unchanged]
        // Find all enemies within range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, range, enemyLayerMask);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("No enemies in range.");
            return;
        }

        // Identify the closest enemy
        float closestDistance = Mathf.Infinity;
        Enemy closestEnemy = null;

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            float distance = Vector2.Distance(player.transform.position, enemyCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyCollider.GetComponent<Enemy>();
            }
        }

        if (closestEnemy != null)
        {
            // Deal damage to the closest enemy
            closestEnemy.TakeDamage(damage);
            Debug.Log($"Bow attacked {closestEnemy.name} for {damage} damage.");
        }
    }

    public override void UpdateWeaponUI(int beatCount)
    {
        // Update the UI indicator for the bow
        if (beatIndicatorManager != null)
        {
            beatIndicatorManager.UpdateBeatCount(beatCount);
        }
        else
        {
            Debug.LogWarning("BeatIndicatorManager not assigned in Bow class.");
        }
        base.UpdateWeaponUI(beatCount);
    }
}