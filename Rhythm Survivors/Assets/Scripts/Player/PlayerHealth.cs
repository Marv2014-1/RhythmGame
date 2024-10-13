using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure TextMeshPro is imported

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public static int currentHealth;

    [Header("UI Elements")]
    public Image healthBarFill; // Assign HealthBarFill Image in the Inspector
    public TextMeshProUGUI healthText; // Assign HealthText (TextMeshPro) in the Inspector

    [Header("Health Bar Animation")]
    public float smoothSpeed = 5f; // Speed of the fill transition

    private float targetFillAmount;

    public Animator anim; // Play death animation upon death

    void Start()
    {
        // Initialize current health
        currentHealth = maxHealth;
        targetFillAmount = 1f;
        UpdateHealthUIImmediate();
    }

    void Update()
    {
        // Temporary testing inputs
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(10);
        }
    }

    /// Applies damage to the player.
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        targetFillAmount = (float)currentHealth / maxHealth;
        StartCoroutine(SmoothFill());

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
    private System.Collections.IEnumerator SmoothFill()
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
        anim.SetBool("IsDead", true);

        Debug.Log("Player has died!");
    }
}
