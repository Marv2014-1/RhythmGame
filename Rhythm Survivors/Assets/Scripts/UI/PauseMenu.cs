using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pauseUI;
    public GameObject optionUI;
    public Slider musicSlider;
    public TMP_Text musicText;
    public float countdownTime;
    private float timeLeft;
    private bool timerOn = false;
    public GameObject countdownTimer;
    public TMP_Text timerTxt;
    private float countdownStart;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (timerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= (Time.realtimeSinceStartup - countdownStart);
                countdownStart = Time.realtimeSinceStartup;
                timerTxt.text = timeLeft.ToString("0.00");
            }
            else
            {
                timerOn = false;
                countdownTimer.SetActive(false);
                Time.timeScale = 1.0f;
                AudioListener.pause = false;
            }
        }
    }

    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
        AudioListener.pause = true;
        timerOn = false;
        countdownTimer.SetActive(false);
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        optionUI.SetActive(false);
        timeLeft = countdownTime;
        countdownStart = Time.realtimeSinceStartup;
        timerOn = true;
        countdownTimer.SetActive(true);
        isPaused = false;
    }

    public void Options()
    {
        pauseUI.SetActive(false);
        optionUI.SetActive(true);
    }

    public void EndRun()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void Back()
    {
        optionUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    public void MusicVolume()
    {
        AudioListener.volume = musicSlider.value;
        float vol = musicSlider.value;
        vol = Mathf.Round(vol * 100);
        musicText.text = vol.ToString();
    }
}
