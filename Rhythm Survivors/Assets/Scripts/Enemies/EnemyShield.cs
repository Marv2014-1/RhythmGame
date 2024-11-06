using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    public float blockChance = 0.5f; // 50% chance to block an attack
    public float blockCooldown = 2f; // Cooldown between blocks
    public int shieldStrength = 100; // Durability of the shield
    public float blockDamageReduction = 1.0f; // Full block when 1.0, partial when <1.0

    // private bool isBlocking = false;
    // private float nextBlockTime = 0f;
    // private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
