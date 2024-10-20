using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    private Vector3 originalLocalPosition; // Store the original local position of the sword
    private Animator animator; // Reference to the Animator component
    private bool isAttacking = false; // Prevents overlapping attacks

    // Reference to the SwordContainer's Transform
    private Transform swordContainerTransform;

    public ParticleSystem attackParticles;
    public AudioSource attackSound;

    protected override void Start()
    {
        base.Start();

        // Store the original local position relative to the SwordContainer
        originalLocalPosition = transform.localPosition;

        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on the Sword object.");
        }

        // Get the SwordContainer's Transform
        if (transform.parent != null)
        {
            swordContainerTransform = transform.parent;
        }
        else
        {
            Debug.LogError("Sword does not have a parent SwordContainer.");
        }

        if (attackParticles == null)
        {
            attackParticles = GetComponentInChildren<ParticleSystem>();
            if (attackParticles == null)
            {
                Debug.LogWarning("No ParticleSystem found for attack effects.");
            }
        }

        if (attackSound == null)
        {
            attackSound = GetComponent<AudioSource>();
            if (attackSound == null)
            {
                Debug.LogWarning("No AudioSource found for attack sounds.");
            }
        }

        if (attackParticles != null)
        {
            attackParticles.Stop();
        }
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Play attack sound
        // attackSound?.Play();

        // Play particle effect
        attackParticles?.Play();

        Vector3 forwardOffset = new Vector3(0f, 0, 0);

        // Calculate left positions relative to SwordContainer
        Vector3 rightPosition = originalLocalPosition + forwardOffset;

        // --- Attack to the Right ---

        // Move to the right side
        transform.localPosition = rightPosition;

        // Play slash animation to the right
        if (animator != null)
        {
            animator.SetTrigger("AttackRight");
        }

        // Wait for the animation to reach the impact point
        yield return new WaitForSeconds(0.25f); // Adjust based on your animation length

        // Wait for the rest of the animation
        // yield return new WaitForSeconds(0.25f);

        // Return to original position

        // Play slash animation to the left
        if (animator != null)
        {
            animator.SetTrigger("AttackLeft");
        }

        // Wait for the animation to reach the impact point
        yield return new WaitForSeconds(0.25f); // Adjust based on your animation length

        // Wait for the rest of the animation
        yield return new WaitForSeconds(0.25f);

        attackParticles?.Stop();

        // Return to original position
        transform.localPosition = originalLocalPosition;

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the Sword hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"Sword hit {enemy.gameObject.name} and dealt {damage} damage.");
            }
        }
    }
}
