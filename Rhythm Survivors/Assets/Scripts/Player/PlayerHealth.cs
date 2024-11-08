using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Required for Coroutines

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Elements")]
    public Image healthBarFill; // Assign HealthBarFill Image in the Inspector
    public TextMeshProUGUI healthText; // Assign HealthText (TextMeshPro) in the Inspector

    [Header("Health Bar Animation")]
    public float smoothSpeed = 5f; // Speed of the fill transition

    private float targetFillAmount;

    public Animator animator; // Play death animation upon death

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f; // Duration of invincibility in seconds
    private bool isInvulnerable = false; // Flag to track invincibility state

    [Header("Knockback Settings")]
    public float knockbackForce = 5f; // Magnitude of the knockback force
    public float knockbackDuration = 0.2f; // Duration of the knockback effect

    private Rigidbody2D rb; // Reference to the player's Rigidbody2D
    private Vector2 knockbackDirection; // Direction of the knockback
    private bool isKnockedBack = false; // Flag to track if the player is being knocked back

    void Start()
    {
        // Initialize current health
        currentHealth = maxHealth;
        targetFillAmount = 1f;
        UpdateHealthUIImmediate();

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on Player.");
        }
    }

    void Update()
    {
        // Temporary testing inputs
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Example: Knockback to the right
            Vector2 knockbackDir = Vector2.right;
            TakeDamage(10, knockbackDir, knockbackForce);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(10);
        }
    }

    /// Applies damage to the player with knockback. If the player is currently invulnerable, damage is ignored.
    public void TakeDamage(int damage, Vector2 knockbackDir, float force)
    {
        if (isInvulnerable)
        {
            // Player is currently invulnerable; ignore damage
            Debug.Log("Player is invulnerable and cannot take damage right now.");
            return;
        }

        // Apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        targetFillAmount = (float)currentHealth / maxHealth;
        animator.SetTrigger("IsHurt"); // Using Trigger instead of Bool for better control
        StartCoroutine(SmoothFill());

        // Apply knockback
        ApplyKnockback(knockbackDir, force);

        // Start invincibility period
        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0)
        {
            UpdateHealthUIImmediate();
            Die();
        }
    }

    /// Heals the player.
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        targetFillAmount = (float)currentHealth / maxHealth;
        StartCoroutine(SmoothFill());

        // Optional: If healing restores some conditions, handle them here
    }

    /// Updates the health bar UI immediately without animation.
    void UpdateHealthUIImmediate()
    {
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    /// Smoothly animates the health bar fill to the target amount.
    private IEnumerator SmoothFill()
    {
        while (!Mathf.Approximately(healthBarFill.fillAmount, targetFillAmount))
        {
            healthBarFill.fillAmount = Mathf.MoveTowards(healthBarFill.fillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
            yield return null;
        }

        // Update text after the fill has reached the target
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    /// Handles player death.
    void Die()
    {
        // Halt player movement
        PlayerMovement.canMove = false;
        // Begin death animation
        animator.SetTrigger("IsDead"); // Using Trigger instead of Bool for better control

        Debug.Log("Player has died!");
    }

    /// Coroutine to handle temporary invincibility after taking damage.
    private IEnumerator InvincibilityCoroutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invincibilityDuration);
        isInvulnerable = false;
    }

    /// Applies a knockback force to the player.
    private void ApplyKnockback(Vector2 direction, float force)
    {
        if (rb != null && !isKnockedBack)
        {
            // Normalize the direction to ensure consistent knockback
            knockbackDirection = direction.normalized;

            // Apply the knockback force using Rigidbody2D's velocity
            rb.velocity = Vector2.zero; // Reset current velocity
            rb.AddForce(knockbackDirection * force, ForceMode2D.Impulse);

            // Start the knockback coroutine to manage knockback duration
            StartCoroutine(KnockbackCoroutine());
        }
    }

    /// Coroutine to handle the duration of the knockback effect.
    /// During knockback, player movement is restricted.
    private IEnumerator KnockbackCoroutine()
    {
        isKnockedBack = true;
        // Disable player movement
        PlayerMovement.canMove = false;

        // Wait for the knockback duration
        yield return new WaitForSeconds(knockbackDuration);

        // Re-enable player movement
        PlayerMovement.canMove = true;
        isKnockedBack = false;
    }

}
