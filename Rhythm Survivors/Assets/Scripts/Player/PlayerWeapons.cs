using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour
{
    public GameObject weaponDisplay, weaponUI;
    public GameObject bow, staff, sword;
    public Sprite bowSprite, staffSprite, swordSprite;

    private List<(string, int)> items = new List<(string, int)>()
    { 
        ("Bow", 0), ("Staff", 0), ("Sword", 0)
    };
    public int maxWeapons;
    public int maxTrinkets;
    private int numWeapons;
    private int numTrinkets;

    // Start is called before the first frame update
    void Start()
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
        // Temporary testing inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (items[FindItem("Bow")].Item2 == 0)
            {
                ProgressItem("Bow");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (items[FindItem("Staff")].Item2 == 0)
            {
                ProgressItem("Staff");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (items[FindItem("Sword")].Item2 == 0)
            {
                ProgressItem("Sword");
            }
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

    private void ProgressItem(string name)
    {
        int index = FindItem(name);

        switch (name)
        {
            case "Bow":
                Instantiate(bow, this.transform);
                AddUIElement(bowSprite, bow.GetComponent<Weapon>().requiredBeats);
                numWeapons++;
                break;
            case "Staff":
                Instantiate(staff, this.transform);
                AddUIElement(staffSprite, staff.GetComponent<Weapon>().requiredBeats);
                numWeapons++;
                break;
            case "Sword":
                Instantiate(sword, this.transform);
                AddUIElement(swordSprite, sword.GetComponent<Weapon>().requiredBeats);
                numWeapons++;
                break;
            default:
                Debug.LogWarning(name + " not found in item list.");
                break;
        }

        items[index] = (name, items[index].Item2 + 1);
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
