using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mainUI;
    public GameObject optionUI;
    public GameObject creditUI;
    public Slider musicSlider;
    public TMP_Text musicText;

    // Loads the game scene
    public void Play()
    {
        SceneManager.LoadScene(sceneName: "Brandon-Test");
    }
    
    // Brings up the options menu
    public void Options()
    {
        mainUI.SetActive(false);
        optionUI.SetActive(true);
    }

    // Brings up the credits screen
    public void Credits()
    {
        mainUI.SetActive(false);
        creditUI.SetActive(true);
    }

    // Exits the application
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // Hides the options menu
    public void OptionBack()
    {
        optionUI.SetActive(false);
        mainUI.SetActive(true);
    }
    
    // Hides the credits screen
    public void CreditBack()
    {
        creditUI.SetActive(false);
        mainUI.SetActive(true);
    }
       
    // Allows the player to adjust the music volume via an onscreen slider
    public void MusicVolume()
    {
        AudioListener.volume = musicSlider.value;
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();
    }
}
