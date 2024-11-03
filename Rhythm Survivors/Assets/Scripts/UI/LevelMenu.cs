using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelMenu : MonoBehaviour
{
    public TMP_Text choice1, choice2, choice3;
    private string item1, item2, item3;
    private PlayerWeapons playerWeapons;

    void Start()
    {
        playerWeapons = FindObjectOfType<PlayerWeapons>();
    }

    public void PopulateUpgrades(List<(string, int)> items)
    {
        // Copy list of player's items
        List<(string, int)> upgrades = new List<(string, int)>(items);

        // Remove items at max level from list
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].Item2 >= 5)
            {
                upgrades.RemoveAt(i);
                i--;
            }
        }

        /*  This block doesn't work for some reason, it says numWeapons and maxWeapons are null, but they're definitely not
        // If player cannot take anymore weapons, remove weapons player doesn't have from list
        if (playerWeapons.numWeapons >= playerWeapons.maxWeapons)
        {
            for (int i = 0; i < upgrades.Count; i++)
            {
                if (upgrades[i].Item2 == 0)
                {
                    upgrades.RemoveAt(i);
                    i--;
                }
            }
        }
        */
        
        // Random roll upgrades from list, if list is exhaused give player choice to heal
        int index = -1;
        for (int i = 0; i < 3; i++)
        {
            if (upgrades.Count != 0)
            {
                index = Random.Range(0, upgrades.Count - 1);
            } else
            {
                index = -1;
            }

            switch (i)
            {
                case 0:
                    if (index != -1)
                    {
                        choice1.text = upgrades[index].Item1 + "\nLv: " + upgrades[index].Item2.ToString();
                        item1 = upgrades[index].Item1;
                    } else
                    {
                        choice1.text = "+100 HP";
                        item1 = "Heal";
                    }                   
                    break;
                case 1:
                    if (index != -1)
                    {
                        choice2.text = upgrades[index].Item1 + "\nLv: " + upgrades[index].Item2.ToString();
                        item2 = upgrades[index].Item1;
                    }
                    else
                    {
                        choice2.text = "+100 HP";
                        item2 = "Heal";
                    }
                    break;
                case 2:
                    if (index != -1)
                    {
                        choice3.text = upgrades[index].Item1 + "\nLv: " + upgrades[index].Item2.ToString();
                        item3 = upgrades[index].Item1;
                    }
                    else
                    {
                        choice3.text = "+100 HP";
                        item3 = "Heal";
                    }
                    break;
                default:
                    Debug.LogWarning("SelectUpgrade loop out of range.");
                    break;
            }

            if (index != -1)
            {
                upgrades.RemoveAt(index);
            }
        }
    }

    public void SelectUpgrade1()
    {
        playerWeapons.CloseMenu(item1);
    }

    public void SelectUpgrade2()
    {
        playerWeapons.CloseMenu(item2);
    }

    public void SelectUpgrade3()
    {
        playerWeapons.CloseMenu(item3);
    }
}
