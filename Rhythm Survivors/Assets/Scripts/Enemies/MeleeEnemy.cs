// using UnityEngine;

// public class MeleeEnemy : Enemy
// {
//     [Header("Crew Attack Settings")]
//     public float attackRange = 3f;
//     public int attackDamage = 10;
//     public float attackCooldown = 2f;

//     private float lastAttackTime;

//     [Header("Attack Hitbox")]
//     public GameObject skeletonHitbox;

//     protected override void Awake()
//     {
//         base.Awake();
//         if (skeletonHitbox != null)
//         {
//             skeletonHitbox.SetActive(false); // Ensure hitbox is initially inactive
//         }
//     }

//     protected override void Update()
//     {
//         base.Update();

//         if (playerTransform == null) return;

//         float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
//         if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
//         {
//             AttemptAttack();
//             lastAttackTime = Time.time;
//         }
//     }

//     private void AttemptAttack()
//     {
//         if (animator == null || skeletonHitbox == null) return;

//         bool isAbove = playerTransform.position.y > transform.position.y;
//         string attackType = isAbove ? "IsAttack1" : "IsAttack2";

//         // Debug to confirm which attack type is being set
//         Debug.Log($"Setting attack trigger: {attackType}");

//         // Reset both triggers to avoid conflicts
//         animator.ResetTrigger("IsAttack1");
//         animator.ResetTrigger("IsAttack2");

//         // Set the trigger for the chosen attack
//         animator.SetTrigger(attackType);

//         // Check if the trigger is successfully set
//         Debug.Log($"Animator trigger {attackType} set.");

//         // Activate hitbox
//         skeletonHitbox.SetActive(true);
//         Invoke(nameof(DisableHitbox), 0.5f); // Adjust timing based on animation length
//     }

//     private void DisableHitbox()
//     {
//         if (skeletonHitbox != null)
//         {
//             skeletonHitbox.SetActive(false);
//         }
//     }
// }





using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;       // Range within which the enemy attacks
    private float nextAttackTime = 0f;

    // public GameObject Hitbox;

    // private MeleeAttack meleeAttack;

    // protected override void Awake()
    // {
    //     base.Awake();
    //     meleeAttack = GetComponent<MeleeAttack>();
    // }
    //   private void Start()
    // {
    //     Hitbox.SetActive(false); // Ensure the hitbox is initially inactive
    // }
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
            }
        }

        // Automatically assign the hitbox if it's not set in the Inspector
        if (hitbox == null)
        {
            hitbox = GetComponentInChildren<Hitbox>();
            if (hitbox == null)
            {
                Debug.LogError("Hitbox not found as a child of MeleeEnemy. Please assign it in the Inspector or ensure it exists as a child GameObject.");
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
                         // Randomly choose one of the three attack directions
        int attackType = UnityEngine.Random.Range(0, 3);
        string triggerName = attackType switch
        {
            0 => "TriggerAttackTop",
            1 => "TriggerAttackSide",
            _ => "TriggerAttackBottom",
        };

        Attack(triggerName, 0.001f, 0.5f);


        // case 0:
        //     Attack("TriggerAttackTop", attackDamage, 0.1f, 0.5f);
        //     break;
        // case 1:
        //     Attack("TriggerAttackSide", attackDamage, 0.1f, 0.5f);
        //     break;
        // case 2:
        //     Attack("TriggerAttackBottom", attackDamage, 0.1f, 0.5f);
        //     break;

    }
    public void Attack(string triggerName, float activateTime, float deactivateTime)
    {
        // currentAttackDamage = damage;       // Set the current attack damage
        animator.SetTrigger(triggerName);    // Trigger the specific attack animation
        hitbox.ActivateHitbox();
        // Invoke("ActivateHitbox", activateTime); // Activate hitbox after `activateTime` seconds
        // Invoke("DeactivateHitbox", 0.2f); // Deactivate hitbox after `deactivateTime` seconds
        Invoke(nameof(ActivateHitbox), activateTime); // Activate hitbox after `activateTime` seconds
        Invoke(nameof(DeactivateHitbox), deactivateTime); // Deactivate hitbox after `deactivateTime` seconds
        // Invoke(nameof(EndAttack), activateTime);
    }


    // Called at the end of the attack animation to reset movement
    public void FinishAttack()
    {
        canMove = true; // Allow movement again
        // animator.SetBool("IsMoving", true); // Reset the attacking parameter
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
