using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelMenu : MonoBehaviour
{
    public TMP_Text choice1, choice2, choice3;
    private string item1, item2, item3;
    private int maxWeapons = 3, numWeapons = 0;
    private PlayerWeapons playerWeapons;

    void Start()
    {
        playerWeapons = FindObjectOfType<PlayerWeapons>();
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public void PopulateUpgrades(List<(string, int)> items)
    {
        // Copy list of player's items
        List<(string, int)> upgrades = new List<(string, int)>(items);

        numWeapons = 0;
        for (int i = 0; i < upgrades.Count; i++)
        {
            // Count number of weapons player has
            if (upgrades[i].Item2 > 0)
            {
                numWeapons++;
            }

            // Remove items at max level from list
            if (upgrades[i].Item2 >= 8)
            {
                upgrades.RemoveAt(i);
                i--;
            }
        }

        /*
         * This bit ensure the first time the player levels up they have to pick a second weapon instead of upgrading their starting weapon
         * For some reason if the game tries to give the option to upgrade their starting weapon the first time they level up the game crashes
         * I have no clue why it happens and I ran out of time to figure it out, sorry -Brandon
         */
        if (numWeapons == 1)
        {
            for (int i = 0; i < upgrades.Count; i++)
            {
                // Remove items at max level from list
                if (upgrades[i].Item2 > 0)
                {
                    upgrades.RemoveAt(i);
                    i--;
                }
            }
        }

        // If player cannot take anymore weapons, remove weapons player doesn't have from list
        if (numWeapons >= maxWeapons)
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
        
        // Random roll upgrades from list, if list is exhaused give player choice to heal
        int index = -1;
        for (int i = 0; i < 3; i++)
        {
            if (upgrades.Count != 0)
            {
                index = Random.Range(0, upgrades.Count);
            } else
            {
                index = -1;
            }

            switch (i)
            {
                case 0:
                    if (index != -1)
                    {
                        if (upgrades[index].Item2 == 0)
                        {
                            choice1.text = "Gain\n" + upgrades[index].Item1;
                        } else
                        {
                            int tmp = upgrades[index].Item2;
                            choice1.text = "Lv: " + tmp.ToString() + " -> " + (tmp + 1).ToString() + "\n" + upgrades[index].Item1;
                            
                            string tmp2 = playerWeapons.CheckUpgrade(upgrades[index].Item1);
                            if (tmp2 != "N/A")
                            {
                                choice1.text += "\n\n" + tmp2;
                            }
                            
                        }
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
                        if (upgrades[index].Item2 == 0)
                        {
                            choice2.text = "Gain\n" + upgrades[index].Item1;
                        }
                        else
                        {
                            int tmp = upgrades[index].Item2;
                            choice2.text = "Lv: " + tmp.ToString() + " -> " + (tmp + 1).ToString() + "\n" + upgrades[index].Item1;
                            
                            string tmp2 = playerWeapons.CheckUpgrade(upgrades[index].Item1);
                            if (tmp2 != "N/A")
                            {
                                choice2.text += "\n\n" + tmp2;
                            }
                            
                        }
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
                        if (upgrades[index].Item2 == 0)
                        {
                            choice3.text = "Gain\n" + upgrades[index].Item1;
                        }
                        else
                        {
                            int tmp = upgrades[index].Item2;
                            choice3.text = "Lv: " + tmp.ToString() + " -> " + (tmp + 1).ToString() + "\n" + upgrades[index].Item1;
                            
                            string tmp2 = playerWeapons.CheckUpgrade(upgrades[index].Item1);
                            if (tmp2 != "N/A")
                            {
                                choice3.text += "\n\n" + tmp2;
                            }
                            
                        }
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
