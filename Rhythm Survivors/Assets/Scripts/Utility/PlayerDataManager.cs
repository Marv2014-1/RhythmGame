using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public ScoreManager scoreManager;
    public MusicPlayer musicPlayer;
    public int bestScore;
    public float musicVol, sfxVol;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    // Saves the player's best score
    public void SaveGame()
    {
        PlayerData playerData = new PlayerData();
        playerData.bestScore = ScoreManager.maxScore;
        playerData.musicVol = musicPlayer.musicVolume;
        playerData.sfxVol = musicPlayer.sfxVolume;

        string json = JsonUtility.ToJson(playerData);
        string path = Application.persistentDataPath + "/playerData.json";
        System.IO.File.WriteAllText(path, json);
    }

    // Loads the player's best score
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
            ScoreManager.maxScore = loadedData.bestScore;
            scoreManager.UpdateMenu();
            musicPlayer.MusicVolume(loadedData.musicVol);
            musicPlayer.SFXVolume(loadedData.sfxVol);
        }
        else
        {
            Debug.LogWarning("File not found!");
        }
    }

    public void ResetGame()
    {
        ScoreManager.maxScore = 0;
        scoreManager.UpdateMenu();
        Debug.Log(ScoreManager.maxScore.ToString());
    }
}
