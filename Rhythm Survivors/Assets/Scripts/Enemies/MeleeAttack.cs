using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    // private Animator animator;
    // private Hitbox hitbox;

    private int currentAttackDamage;
    private float attackDuration = 1.0f; // Set a default value for attack duration

    private void Start()
    {
        // animator = GetComponent<Animator>();
        // Hitbox = transform.Find("Hitbox").gameObject;
        // if (hitbox != null)
        // {
        //     hitbox.DeactivateHitbox();
        // }
    }

    private void Update()
    {

    }

    // public void Attack(string triggerName, int damage, float activateTime, float deactivateTime)
    // {
    //     currentAttackDamage = damage;       // Set the current attack damage
    //     // animator.SetTrigger(triggerName);    // Trigger the specific attack animation
    //     hitbox.ActivateHitbox();
    //     // Invoke("ActivateHitbox", activateTime); // Activate hitbox after `activateTime` seconds
    //     // Invoke("DeactivateHitbox", 0.2f); // Deactivate hitbox after `deactivateTime` seconds
    //     Invoke(nameof(EndAttack), activateTime);  
    // }

    // void ActivateHitbox()
    // {
    //     if (hitbox != null)
    //         hitbox.enabled = true;
    // }

    // void DeactivateHitbox()
    // {
    //     if (Hitbox != null)
    //         Hitbox.SetActive(false);
    // }

    // public int GetAttackDamage()
    // {
    //     return currentAttackDamage;
    // }
    // private void EndAttack()
    // {
    //     hitbox.DeactivateHitbox();
    //     // isAttacking = false;
    // }
}
