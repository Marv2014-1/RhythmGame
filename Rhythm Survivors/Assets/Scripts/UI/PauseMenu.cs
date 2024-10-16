using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public static bool isAlive = true;
    public GameObject pauseUI;
    public GameObject optionUI;
    public GameObject endUI;
    public Slider musicSlider, sfxSlider;
    public TMP_Text musicText, sfxText;
    public float countdownTime;
    private float timeLeft;
    private bool timerOn = false;
    public GameObject countdownTimer;
    public TMP_Text timerTxt;
    private float countdownStart;
    public ScoreManager score;

    void Start()
    {
        // Ensure player is alive and game is unpaused at game start
        isPaused = false;
        isAlive = true;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        pauseUI.SetActive(false);
        optionUI.SetActive(false);
        endUI.SetActive(false);

        // Find score manager
        score = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is currently alive
        if (isAlive)
        {
            // Check if pause key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Pause or unpause game
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }

            // Check if the cooldown to begin the game after unpausing is running
            if (timerOn)
            {
                if (timeLeft > 0)
                {
                    // Runs down the cooldown timer
                    timeLeft -= (Time.realtimeSinceStartup - countdownStart);
                    countdownStart = Time.realtimeSinceStartup;
                    timerTxt.text = timeLeft.ToString("0.00");
                }
                else
                {
                    // Continues gameplay
                    timerOn = false;
                    countdownTimer.SetActive(false);
                    Time.timeScale = 1.0f;
                    AudioListener.pause = false;
                }
            }
        }
        else
        {
            // The player is dead and the game is over
            EndRun();
        }
    }

    // Pauses the game and brings up the pause UI
    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
        AudioListener.pause = true;
        timerOn = false;
        countdownTimer.SetActive(false);
    }

    // Hides all the pause UI and begins countdown to resume gameplay
    void Resume()
    {
        pauseUI.SetActive(false);
        optionUI.SetActive(false);
        timeLeft = countdownTime;
        countdownStart = Time.realtimeSinceStartup;
        timerOn = true;
        countdownTimer.SetActive(true);
        isPaused = false;
    }

    // Opens the options menu from the pause menu
    void Options()
    {
        pauseUI.SetActive(false);
        optionUI.SetActive(true);

        musicSlider.value = MusicPlayer.music.volume;
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();

        sfxSlider.value = MusicPlayer.sfx.volume;
        vol = sfxSlider.value;
        vol = Mathf.Round(vol * 100);
        sfxText.text = vol.ToString();

        score.Hide();
    }

    // Ends the game and brings up the end UI
    void EndRun()
    {
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        pauseUI.SetActive(false);
        endUI.SetActive(true);
    }

    // Loads the Main Menu scene
    void MainMenu()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }

    // Exits the application
    void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // Returns from the options menu to the pause menu
    void Back()
    {
        optionUI.SetActive(false);
        pauseUI.SetActive(true);
        score.GameShow();
    }

    // Allows the player to adjust the music volume via an onscreen slider
    public void MusicSlider()
    {
        MusicPlayer.MusicVolume(musicSlider.value);
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();
    }

    // Allows the player to adjust the sound effect volume via an onscreen slider
    public void SFXSlider()
    {
        MusicPlayer.SFXVolume(sfxSlider.value);
        float vol = sfxSlider.value;
        vol = Mathf.Round(vol * 100);
        sfxText.text = vol.ToString();
    }
}
