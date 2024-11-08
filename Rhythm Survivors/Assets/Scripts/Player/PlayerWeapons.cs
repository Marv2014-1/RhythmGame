using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour
{
    public GameObject weaponDisplay, weaponUI, levelMenu;
    public GameObject bow, staff, sword, spear;
    public Sprite bowSprite, staffSprite, swordSprite, spearSprite;

    private List<(string, int)> items = new List<(string, int)>()
    { 
        ("Bow", 0), ("Staff", 0), ("Sword", 0), ("Spear", 0)
    };
    private int numWeapons = 0;

    // Start is called before the first frame update
    void Start()
    {
        levelMenu.SetActive(false);
        ProgressItem("Staff");
    }

    // Update is called once per frame
    void Update()
    {
        // Temporary testing inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OpenMenu();
        }
    }

    private int FindItem(string name)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Item1 == name)
            {
                return i;
            }
        }
        return -1;
    }

    public void OpenMenu()
    {
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        levelMenu.GetComponent<LevelMenu>().PopulateUpgrades(items);
        levelMenu.SetActive(true);
    }

    public void CloseMenu(string name)
    {
        AudioListener.pause = false;
        ProgressItem(name);
        levelMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public string CheckUpgrade(string name)
    {
        string statUpgrade = "N/A";

        switch (name)
        {
            case "Bow":
                statUpgrade = bow.GetComponent<Weapon>().ShowUpgrade();
                break;
            case "Staff":
                statUpgrade = staff.GetComponent<Weapon>().ShowUpgrade();
                break;
            case "Sword":
                statUpgrade = sword.GetComponent<Weapon>().ShowUpgrade();
                break;
            case "Spear":
                statUpgrade = spear.GetComponent<Weapon>().ShowUpgrade();
                break;
            default:
                Debug.LogWarning(name + " not found in item list.");
                break;
        }

        return statUpgrade;
    }

    private void ProgressItem(string name)
    {
        int index = FindItem(name);

        switch (name)
        {
            case "Heal":
                gameObject.GetComponent<PlayerHealth>().Heal(100);
                break;
            case "Bow":
                if (items[index].Item2 == 0)
                {
                    bow = Instantiate(bow, this.transform);
                    AddUIElement(bowSprite, bow.GetComponent<Weapon>().requiredBeats);
                    numWeapons++;
                }
                else
                {
                    bow.GetComponent<Weapon>().UpgradeWeapon();
                }
                break;
            case "Staff":
                if (items[index].Item2 == 0)
                {
                    staff = Instantiate(staff, this.transform);
                    AddUIElement(staffSprite, staff.GetComponent<Weapon>().requiredBeats);
                    numWeapons++;
                }
                else
                {
                    staff.GetComponent<Weapon>().UpgradeWeapon();
                }
                break;
            case "Sword":
                if (items[index].Item2 == 0)
                {
                    sword = Instantiate(sword, this.transform);
                    AddUIElement(swordSprite, sword.GetComponent<Weapon>().requiredBeats);
                    numWeapons++;
                } 
                else
                {
                    sword.GetComponent<Weapon>().UpgradeWeapon();
                }
                break;
            case "Spear":
                if(items[index].Item2 == 0)
                {
                    spear = Instantiate(spear, this.transform);
                    AddUIElement(spearSprite, spear.GetComponent<Weapon>().requiredBeats);
                    numWeapons++;
                } 
                else
                {
                    spear.GetComponent<Weapon>().UpgradeWeapon();
                }
                break;
            default:
                Debug.LogWarning(name + " not found in item list.");
                break;
        }

        if (index != -1)
        {
            items[index] = new(items[index].Item1, items[index].Item2 + 1);
        }
    }

    private void AddUIElement(Sprite weaponSprite, int beats)
    {
        GameObject newUI = Instantiate(weaponUI, weaponDisplay.transform);
        newUI.GetComponent<RectTransform>().anchoredPosition += Vector2.left * (newUI.GetComponent<RectTransform>().rect.width * numWeapons);
        newUI.GetComponentInChildren<WeaponBeatIndicatorManager>().requiredBeats = beats;
        newUI = newUI.transform.GetChild(0).gameObject;
        newUI.GetComponent<Image>().sprite = weaponSprite;
    }
}
