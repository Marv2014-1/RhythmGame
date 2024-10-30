using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Weapon
{
    [Header("Staff Settings")]
    public GameObject sparkPrefab;          // Assign your Spark prefab in the Inspector
    public Transform sparkSpawnPoint;       // The center point from where sparks are emitted
    public int numberOfSparks = 8;          // Adjustable number of sparks
    public float sparkSpeed = 5f;           // Speed at which sparks travel
    public float sparkRange = 10f;          // Distance after which sparks despawn

    protected override void Start()
    {
        base.Start();

        // Ensure sparkSpawnPoint is assigned
        if (sparkSpawnPoint == null)
        {
            Debug.LogError("SparkSpawnPoint is not assigned in the Staff class.");
        }
    }

    public override void Attack()
    {
        float angleStep = 360f / numberOfSparks;
        float angle = 0f;

        for (int i = 0; i < numberOfSparks; i++)
        {
            // Calculate the direction for each spark
            float sparkDirX = Mathf.Cos((angle * Mathf.PI) / 180f);
            float sparkDirY = Mathf.Sin((angle * Mathf.PI) / 180f);

            Vector3 sparkMoveVector = new Vector3(sparkDirX, sparkDirY, 0f);
            Vector2 sparkDirection = sparkMoveVector.normalized;

            // Instantiate the spark
            GameObject sparkInstance = Instantiate(sparkPrefab, sparkSpawnPoint.position, Quaternion.identity);
            Spark spark = sparkInstance.GetComponent<Spark>();

            if (spark != null)
            {
                spark.SetDamage(damage);
                spark.SetSpeed(sparkSpeed);
                spark.SetRange(sparkRange);
                spark.SetDirection(sparkDirection);
            }
            else
            {
                Debug.LogError("Spark prefab does not have a Spark component.");
            }

            angle += angleStep;
        }
    }
}
