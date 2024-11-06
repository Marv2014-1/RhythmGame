using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    private int level, experience, xpToLevel;
    float levelMultiplier = 2f;

    public Image xpBarFill; // Assign ExperienceBarFill Image in the Inspector
    public float smoothSpeed = 5f; // Speed of the fill transition
    private float targetFillAmount;

    private PlayerWeapons playerWeapons;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        experience = 0;
        xpToLevel = 40;
        UpdateXPUIImmediate();
        playerWeapons = gameObject.GetComponent<PlayerWeapons>();
    }

    // Gives the player experience from defeated enemies
    public void GetExperience(int exp)
    {
        experience += exp;

        if (experience >= xpToLevel)
        {
            LevelUp();
            experience -= xpToLevel;
            xpToLevel = (int)(xpToLevel * levelMultiplier);
            levelMultiplier -= 0.1f;
            if (levelMultiplier < 1.1f)
            {
                levelMultiplier = 1.1f;
            }
            GetExperience(0);
        }
        else
        {
            targetFillAmount = (float)experience / xpToLevel;
            StartCoroutine(SmoothFill());
        }
    }

    // Levels up the player and informs the weapon script to prompt an upgrade
    void LevelUp()
    {
        playerWeapons.OpenMenu();
        level += 1;
    }

    /// Updates the health bar UI immediately without animation.
    void UpdateXPUIImmediate()
    {
        xpBarFill.fillAmount = (float)experience / xpToLevel;
    }

    /// Smoothly animates the health bar fill to the target amount.
    private System.Collections.IEnumerator SmoothFill()
    {
        while (!Mathf.Approximately(xpBarFill.fillAmount, targetFillAmount))
        {
            xpBarFill.fillAmount = Mathf.MoveTowards(xpBarFill.fillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
