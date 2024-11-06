using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Settings")]
    public GameObject slashPrefab; // Assign the Slash prefab in the Inspector
    public Transform slashSpawnPointR; // Assign the spawn point in the Inspector
    public Transform slashSpawnPointL; // Assign the flipped spawn point in the Inspector

    protected override void Start()
    {
        base.Start();

        upgrades = new List<(string, int)>()
        {
            ("Knockback", 5), ("Damage", 10), ("Knockback", 5), ("Damage", 10)
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
            slash.SetKnockback(knockback);

            if (player.transform.GetChild(0).transform.localScale.x > 0)
            {
                slash.SetTransform(slashSpawnPointR);
            } else
            {
                slash.SetTransform(slashSpawnPointL);
            }
        }
        else
        {
            Debug.LogError("Slash prefab does not have an Slash component.");
        }
    }
}
