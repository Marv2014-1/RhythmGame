using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public int level;
    public int experience;
    public int xpToLevel;
    public Image xpBarFill; // Assign ExperienceBarFill Image in the Inspector
    public float smoothSpeed = 5f; // Speed of the fill transition
    private float targetFillAmount;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        experience = 0;
        xpToLevel = 20;
        UpdateXPUIImmediate();
    }

    // Gives the player experience from defeated enemies
    public void GetExperience(int exp)
    {
        experience += exp;

        if (experience >= xpToLevel)
        {
            LevelUp();
            experience -= xpToLevel;
            xpToLevel *= 2;
            GetExperience(0);
        } else
        {
            targetFillAmount = (float)experience / xpToLevel;
            StartCoroutine(SmoothFill());
        }
    }

    // Levels up the player and informs the weapon script to prompt an upgrade
    void LevelUp()
    {
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
