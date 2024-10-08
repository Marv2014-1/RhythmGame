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

    public void Play()
    {
        SceneManager.LoadScene(sceneName: "Brandon-Test");
    }
    
    public void Options()
    {
        mainUI.SetActive(false);
        optionUI.SetActive(true);
    }

    public void Credits()
    {
        mainUI.SetActive(false);
        creditUI.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void OptionBack()
    {
        optionUI.SetActive(false);
        mainUI.SetActive(true);
    }

    public void CreditBack()
    {
        creditUI.SetActive(false);
        mainUI.SetActive(true);
    }
       

    public void MusicVolume()
    {
        AudioListener.volume = musicSlider.value;
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();
    }
}
