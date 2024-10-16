using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip menuSong, storySong;
    public GameObject musicSource, sfxSource;
    public static MusicPlayer instance;
    public static AudioSource music;
    public static AudioSource sfx;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // Allows the music player to listen for scene changes
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        music = musicSource.GetComponent<AudioSource>();
        sfx = sfxSource.GetComponent<AudioSource>();
    }

    // Calls everytime a scene changes and changes the music track
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        string sceneName = scene.name;

        if (sceneName == "MainMenu")
        {
            Debug.Log("MainMenu Found");
            music.clip = menuSong;
        }
        else if (sceneName == "Story")
        {
            music.clip = storySong;
        } else
        {
            music.clip = null;
        }

        AudioListener.pause = false;
        music.Play();
    }

    public static void MasterVolume(float volume)
    {
        //musicVolume = volume;
        // masterGroup.volume = volume;
    }

    public static void MusicVolume(float volume)
    {
        music.volume = volume;
    }

    public static void SFXVolume(float volume)
    {
        sfx.volume = volume;
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
