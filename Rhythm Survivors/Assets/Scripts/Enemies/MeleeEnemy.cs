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
    [Header("Crew Attack Settings")]
    public float attackRange = 3f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    private float lastAttackTime;

    [Header("Attack Hitbox")]
    public GameObject skeletonHitbox;

    protected override void Awake()
    {
        base.Awake();

        // Debug to confirm skeletonHitbox setup
        if (skeletonHitbox != null)
        {
            skeletonHitbox.SetActive(false); // Ensure hitbox is initially inactive
            Debug.Log("skeletonHitbox initialized and set to inactive.");
        }
        else
        {
            Debug.LogWarning("skeletonHitbox is not assigned in MeleeEnemy.");
        }

        // Debug to confirm animator setup
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log("Animator component found on MeleeEnemy.");
            }
            else
            {
                Debug.LogError("Animator component is missing on MeleeEnemy.");
            }
        }

        // Debug to confirm playerTransform setup
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform not assigned.");
        }
        else
        {
            Debug.Log("Player Transform found and assigned.");
        }
    }

    protected override void Update()
    {
        base.Update();

        // Debug playerTransform check in Update
        if (playerTransform == null)
        {
            Debug.LogWarning("playerTransform is null in Update.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Debug.Log($"Distance to player: {distanceToPlayer}");

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttemptAttack();
            lastAttackTime = Time.time;
            Debug.Log("AttemptAttack called.");
        }
    }

    private void AttemptAttack()
    {
        // Debug components before executing attack logic
        if (animator == null)
        {
            Debug.LogWarning("Animator is null in AttemptAttack.");
            return;
        }
        if (skeletonHitbox == null)
        {
            Debug.LogWarning("skeletonHitbox is null in AttemptAttack.");
            return;
        }

        bool isAbove = playerTransform.position.y > transform.position.y;
        string attackType = isAbove ? "IsAttack1" : "IsAttack2";

        // Debug to confirm which attack type is being set
        Debug.Log($"Setting attack trigger: {attackType}");

        // Reset both triggers to avoid conflicts
        animator.ResetTrigger("IsAttack1");
        animator.ResetTrigger("IsAttack2");

        // Set the trigger for the chosen attack
        animator.SetTrigger(attackType);
        Debug.Log($"Animator trigger {attackType} set.");

        // Activate hitbox
        skeletonHitbox.SetActive(true);
        Debug.Log("skeletonHitbox activated.");
        Invoke(nameof(DisableHitbox), 0.5f); // Adjust timing based on animation length
    }

    private void DisableHitbox()
    {
        if (skeletonHitbox != null)
        {
            skeletonHitbox.SetActive(false);
            Debug.Log("skeletonHitbox deactivated.");
        }
        else
        {
            Debug.LogWarning("skeletonHitbox is null in DisableHitbox.");
        }
    }
}
