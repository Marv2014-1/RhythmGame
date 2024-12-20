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
    public Slider sfxSlider;
    public TMP_Text musicText;
    public TMP_Text sfxText;
    public ScoreManager score;
    public MusicPlayer musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        score = FindObjectOfType<ScoreManager>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    // Loads the game scene
    void Play()
    {
        SceneManager.LoadScene(sceneName: "Story");
    }
    
    // Brings up the options menu
    void Options()
    {
        mainUI.SetActive(false);
        optionUI.SetActive(true);
        score.Hide();


        musicSlider.value = musicPlayer.musicVolume;
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();

        sfxSlider.value = musicPlayer.sfxVolume;
        vol = sfxSlider.value;
        vol = Mathf.Round(vol * 100);
        sfxText.text = vol.ToString();
    }

    // Brings up the credits screen
    void Credits()
    {
        mainUI.SetActive(false);
        creditUI.SetActive(true);
        score.Hide();
    }

    // Exits the application
    void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // Hides the options menu
    void OptionBack()
    {
        optionUI.SetActive(false);
        mainUI.SetActive(true);
        score.MenuShow();
    }
    
    // Hides the credits screen
    void CreditBack()
    {
        creditUI.SetActive(false);
        mainUI.SetActive(true);
        score.MenuShow();
    }
       
    // Allows the player to adjust the music volume via an onscreen slider
    public void MusicSlider()
    {
        musicPlayer.MusicVolume(musicSlider.value);
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();
    }

    // Allows the player to adjust the sound effect volume via an onscreen slider
    public void SFXSlider()
    {
        musicPlayer.SFXVolume(sfxSlider.value);
        float vol = sfxSlider.value;
        vol = Mathf.Round(vol * 100);
        sfxText.text = vol.ToString();
    }

    public void ReloadSliders()
    {
        musicSlider.value = musicPlayer.musicVolume;
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();

        sfxSlider.value = musicPlayer.sfxVolume;
        vol = sfxSlider.value;
        vol = Mathf.Round(vol * 100);
        sfxText.text = vol.ToString();
    }
}
