using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Settings")]
    public GameObject slashPrefab; // Assign the Slash prefab in the Inspector
    public Transform slashSpawnPoint; // Assign the spawn point in the Inspector

    protected override void Start()
    {
        base.Start();

        upgrades = new List<(string, int)>()
        {
            ("Knockback", 5), ("Knockback", 5), ("Knockback", 5), ("Damage", 5)
        };
    }

    public override void Attack()
    {
        // Instantiate a slash at the spawn point
        GameObject slashInstance = Instantiate(slashPrefab, player.transform);

        Slash slash = slashInstance.GetComponent<Slash>();

        if (slash != null)
        {
            slash.SetDamage(damage);
            slash.SetTransform(slashSpawnPoint);
            slash.SetKnockback(knockback);
        }
        else
        {
            Debug.LogError("Slash prefab does not have an Slash component.");
        }
    }
}
