using System.Collections.Generic;
using UnityEngine;

public class Staff : Weapon
{
    [Header("Staff Settings")]
    public GameObject sparkPrefab;          // Assign your Spark prefab in the Inspector

    protected override void Start()
    {
        base.Start();

        upgrades = new List<(string, int)>()
        {
            ("Damage", 5), ("Projectile", 3), ("Range", 5), ("Projectile", 3)
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
            GameObject sparkInstance = Instantiate(sparkPrefab, this.transform);
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

            sparkInstance.transform.SetParent(null, true);
            angle += angleStep;
        }
    }
}
