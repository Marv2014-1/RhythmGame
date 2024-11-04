using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    protected int level;
    public int damage, pierce, projectile;
    public float range, knockback, speed;
    public int requiredBeats; // Number of beats required to activate attack
    public GameObject player; // Reference to the player object
    public LayerMask enemyLayerMask; // Layer mask to identify enemies
    protected int beatCount = 0; // Individual beat counter for the weapon
    public WeaponBeatIndicatorManager weaponBeatIndicatorManager; // Reference to the UI indicator manager
    public BeatDetector beatDetector; // Reference to the BeatDetector

    protected List<(string, int)> upgrades;

    protected virtual void Start()
    {
        level = 1;

        // Assign the player object
        player = GameObject.Find("Player");

        // Assign the beat detector
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        beatDetector = musicPlayer.GetComponent<BeatDetector>();

        // Subscribe to the OnBeatHit event
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.AddListener(OnBeatDetected);
        }
        else
        {
            Debug.LogError($"BeatDetector reference not assigned in {weaponName} class.");
        }
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.RemoveListener(OnBeatDetected);
        }
    }

    // This method will be called when a beat is detected
    protected virtual void OnBeatDetected()
    {
        beatCount++;
        UpdateWeaponUI(beatCount);

        if (beatCount >= requiredBeats)
        {
            Attack();
            beatCount = 0; // Reset beat count after attack
            UpdateWeaponUI(beatCount);
        }
    }

    // This method will be implemented by each specific weapon type
    public abstract void Attack();

    // Update Weapon UI with the current beat count
    public virtual void UpdateWeaponUI(int beatCount)
    {
        if (weaponBeatIndicatorManager != null)
        {
            weaponBeatIndicatorManager.UpdateBeatCount(beatCount);
        }
        Debug.Log($"Weapon UI updated with beat count: {beatCount}");
    }

    public virtual void UpgradeWeapon()
    {
        string attribute = upgrades[level - 1].Item1;
        int value = upgrades[level - 1].Item2;
        switch (attribute)
        {
            case "Damage":
                damage += value;
                break;
            case "Range":
                range += (float)value;
                break;
            case "Knockback":
                knockback += (float)value;
                break;
            case "Speed":
                speed += (float)value;
                break;
            case "Pierce":
                pierce += value;
                break;
            case "Projectile":
                projectile += value;
                break;
            case "Beat":
                requiredBeats += value;
                break;
            default:
                Debug.LogWarning("Upgrade attribute not found.");
                break;
        }

        level += 1;
    }
}

