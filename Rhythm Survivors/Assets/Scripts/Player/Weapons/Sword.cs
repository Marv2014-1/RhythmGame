using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Settings")]
    public GameObject slashPrefab; // Assign the Slash prefab in the Inspector
    public Transform slashSpawnPoint; // Assign the spawn point in the Inspector

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        // Instantiate a slash at the spawn point
        GameObject slashInstance = Instantiate(slashPrefab, slashSpawnPoint.position, transform.rotation, this.transform);

        Slash slash = slashInstance.GetComponent<Slash>();

        if (slash != null)
        {
            slash.SetDamage(damage);
            slash.SetTransform(slashSpawnPoint);
        }
        else
        {
            Debug.LogError("Slash prefab does not have an Slash component.");
        }
    }
}
