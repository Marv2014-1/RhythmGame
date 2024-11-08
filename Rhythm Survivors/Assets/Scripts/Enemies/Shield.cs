using UnityEngine;

public class Shield : MonoBehaviour
{
    [Range(0f, 1f)]
    public float blockChance = 0.5f;  // Probability of blocking an attack (0 = never, 1 = always)
    public int blockStrength = 50;    // Amount of damage reduced on a successful block

    /// <summary>
    /// Attempts to block and reduce incoming damage. Returns the adjusted damage.
    /// </summary>
    /// <param name="incomingDamage">The original incoming damage value.</param>
    /// <returns>The damage amount after block calculations.</returns>
    public int CalculateDamageAfterBlock(int incomingDamage)
    {
        if (IsBlocking())
        {
            Debug.Log("Shield blocked the attack!");
            return Mathf.Max(incomingDamage - blockStrength, 0);  // Reduce damage, ensuring it’s not below zero
        }
        return incomingDamage;  // No block, return full damage
    }

    /// <summary>
    /// Determines if the shield successfully blocks an attack based on blockChance.
    /// </summary>
    /// <returns>True if the shield blocks, otherwise false.</returns>
    public bool IsBlocking()
    {
        return Random.value <= blockChance;
    }
}
