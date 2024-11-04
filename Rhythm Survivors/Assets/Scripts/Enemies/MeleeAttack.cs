using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D hitbox;
    private int currentAttackDamage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        hitbox = transform.Find("HitboxGameObjectName").GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        
    }

    public void ExecuteAttack(string triggerName, int damage, float activateTime, float deactivateTime)
    {
        currentAttackDamage = damage;       // Set the current attack damage
        animator.SetTrigger(triggerName);    // Trigger the specific attack animation
        Invoke("ActivateHitbox", activateTime); // Activate hitbox after `activateTime` seconds
        Invoke("DeactivateHitbox", deactivateTime); // Deactivate hitbox after `deactivateTime` seconds
    }

    void ActivateHitbox()
    {
        hitbox.gameObject.SetActive(true);
    }

    void DeactivateHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }

    public int GetAttackDamage()
    {
        return currentAttackDamage;
    }
}
