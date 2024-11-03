using System.Collections.Generic;
using UnityEngine;

public class Staff : Weapon
{
    [Header("Staff Settings")]
    public GameObject sparkPrefab;          // Assign your Spark prefab in the Inspector
    public Transform sparkSpawnPoint;       // The center point from where sparks are emitted

    protected override void Start()
    {
        base.Start();

        // Ensure sparkSpawnPoint is assigned
        if (sparkSpawnPoint == null)
        {
            Debug.LogError("SparkSpawnPoint is not assigned in the Staff class.");
        }

        upgrades = new List<(string, int)>()
        {
            ("Range", 5), ("Projectile", 1), ("Damage", 5), ("Projectile", 1)
        };
    }

    public override void Attack()
    {
        float angleStep = 360f / projectile;
        float angle = 0f;

        for (int i = 0; i < projectile; i++)
        {
            // Calculate the direction for each spark
            float sparkDirX = Mathf.Cos((angle * Mathf.PI) / 180f);
            float sparkDirY = Mathf.Sin((angle * Mathf.PI) / 180f);

            Vector3 sparkMoveVector = new Vector3(sparkDirX, sparkDirY, 0f);
            Vector2 sparkDirection = sparkMoveVector.normalized;

            // Instantiate the spark
            GameObject sparkInstance = Instantiate(sparkPrefab, sparkSpawnPoint.position, Quaternion.identity, player.transform);
            Spark spark = sparkInstance.GetComponent<Spark>();

            if (spark != null)
            {
                spark.SetDamage(damage);
                spark.SetSpeed(speed);
                spark.SetRange(range);
                spark.SetDirection(sparkDirection);
                spark.SetKnockback(knockback);
            }
            else
            {
                Debug.LogError("Spark prefab does not have a Spark component.");
            }

            angle += angleStep;
        }
    }
}
