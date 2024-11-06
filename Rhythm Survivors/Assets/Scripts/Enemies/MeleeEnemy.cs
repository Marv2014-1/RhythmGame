using System;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;       
    private float nextAttackTime = 0f;


    [SerializeField] private Hitbox hitbox;

    protected override void Awake()
    {
        base.Awake();

        // Ensure the animator is initialized
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator not found on MeleeEnemy or its children.");
                Die();
            }
        }

        // Automatically assign the hitbox if it's not set in the Inspector
        if (hitbox == null)
        {
            hitbox = GetComponentInChildren<Hitbox>();
            if (hitbox == null)
            {
                Debug.LogError("Hitbox not found as a child of MeleeEnemy. Please assign it in the Inspector or ensure it exists as a child GameObject.");
                Die();
            }
        }
    }
    protected override void Update()
    {
        base.Update();

        if (playerTransform == null) return;

        // Calculate distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // If the player is within attack range and cooldown has passed
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown; // Reset attack cooldown
        }
        else
        {
            MoveTowardsPlayer(); // Continue moving towards player if out of range
        }
    }

    private void PerformAttack()
    {
        if (animator == null)
        {
            Debug.LogWarning("animator is null");
            return;
        }
        if (hitbox == null)
        {
            Debug.LogWarning("hitbox is null");
            return;
        }
        canMove = false; // Stop moving when attacking
                         
        Attack(CheckP(), 0.001f, 0.5f);
    }

    private String CheckP()
    {
        String position = "";
        // Calculate the direction vector from the enemy to the player
        Vector2 directionToPlayer = playerTransform.position - transform.position;

        // Determine if the player is above, below, to the left, or to the right
        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
        {
            // The player is more to the left or right
            if (directionToPlayer.x > 0)
            {
                position = "TriggerAttackSide";
                // Trigger right-side attack or movement logic
            }
            else
            {
                position = "TriggerAttackSide";
                // Trigger left-side attack or movement logic
            }
        }
        else
        {
            // The player is more above or below
            if (directionToPlayer.y > 0)
            {
                position = "TriggerAttackTop";
                // Trigger upward attack or movement logic
            }
            else
            {
                position = "TriggerAttackBottom";
                // Trigger downward attack or movement logic
            }
        }
        return position;
    }
    public void Attack(string triggerName, float activateTime, float deactivateTime)
    {

        animator.SetTrigger(triggerName);    
        Invoke(nameof(ActivateHitbox), activateTime); 
        hitbox.ActivateHitbox();
        // Invoke("ActivateHitbox", activateTime); // Activate hitbox after `activateTime` seconds
        // Invoke("DeactivateHitbox", 0.2f); // Deactivate hitbox after `deactivateTime` seconds

        Invoke(nameof(DeactivateHitbox), deactivateTime); // Deactivate hitbox after `deactivateTime` seconds
        // Invoke(nameof(EndAttack), activateTime);
    }


    // Called at the end of the attack animation to reset movement
    public void FinishAttack()
    {
        canMove = true; // Resume moving after attacking
        
    }

    private void ActivateHitbox()
    {
        hitbox?.ActivateHitbox();  // Activate the hitbox
    }

    private void DeactivateHitbox()
    {
        hitbox?.DeactivateHitbox();  // Deactivate the hitbox
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}
