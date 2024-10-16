using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public static int currentScore, maxScore;
    public GameObject menuScore, gameScore;
    public TMP_Text menuText, gameText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentScore = 0;
        maxScore = 0;
    }

    // Allows the music player to listen for scene changes
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Calls everytime a scene changes and changes the music track
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "MainMenu")
        {
            MenuShow();

            if (currentScore > maxScore)
            {
                maxScore = currentScore;
            }

            menuText.text = "Best Score: " + maxScore.ToString();
        }
        else if (sceneName == "GameStart")
        {
            GameShow();
            ZeroScore();
        }
        else
        {
            Hide();
        }
    }

    public void UpdateScore(int score)
    {
        currentScore += score;
        gameText.text = "Score: " + currentScore.ToString();
    }

    void ZeroScore()
    {
        currentScore = 0;
        gameText.text = "Score: " + currentScore.ToString();
    }

    public void Hide()
    {
        menuScore.SetActive(false);
        gameScore.SetActive(false);
    }

    public void MenuShow()
    {
        menuScore.SetActive(true);
        gameScore.SetActive(false);
    }

    public void GameShow()
    {
        menuScore.SetActive(false);
        gameScore.SetActive(true);
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
