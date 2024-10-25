using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour
{
    public GameObject weaponDisplay, weaponUI;
    public GameObject bow, staff, sword;
    public Sprite bowSprite, staffSprite, swordSprite;

    private List<string> weapons = new List<string>(); // List of weapons
    private int num = 0;

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
            if (!weapons.Contains("Bow"))
            {
                weapons.Add("Bow");
                Instantiate(bow, this.transform);
                AddUIElement(bowSprite, bow.GetComponent<Weapon>().requiredBeats);
                num++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!weapons.Contains("Staff"))
            {
                weapons.Add("Staff");
                Instantiate(staff, this.transform);
                AddUIElement(staffSprite, staff.GetComponent<Weapon>().requiredBeats);
                num++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!weapons.Contains("Sword"))
            {
                weapons.Add("Sword");
                Instantiate(sword, this.transform);
                AddUIElement(swordSprite, sword.GetComponent<Weapon>().requiredBeats);
                num++;
            }
        }
    }

    private void AddUIElement(Sprite weaponSprite, int beats)
    {
        GameObject newUI = Instantiate(weaponUI, weaponDisplay.transform);
        newUI.GetComponent<RectTransform>().anchoredPosition += Vector2.left * (newUI.GetComponent<RectTransform>().rect.width * num);
        newUI.GetComponentInChildren<WeaponBeatIndicatorManager>().requiredBeats = beats;
        newUI = newUI.transform.GetChild(0).gameObject;
        newUI.GetComponent<Image>().sprite = weaponSprite;
    }
}
