using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Story : MonoBehaviour
{
    public GameObject curtains;
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject BG1;
    public GameObject BG2;
    public TMP_Text storyText;
    private static int step;

    // Start is called before the first frame update
    void Start()
    {
        curtains.SetActive(true);
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        BG1.SetActive(true);
        BG2.SetActive(false);
        step = 1;
        storyText.text = "In the faraway kingdom of Oratorio lived the humble wizard Giocoso who used his musical magic to aid those in need and spread love and song to all he would come across.";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Progress();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(sceneName: "MainMenu");
        }
    }

    void Progress()
    {
        switch (step)
        {
            case 1:
                page1.SetActive(false);
                page2.SetActive(true);
                storyText.text = "However one day in their travels his apprentice Coda betrayed Giocoso, stealing the bulk of his magic for his own ill intentions.";
                break;
            case 2:
                BG1.SetActive(false);
                BG2.SetActive(true);
                page2.SetActive(false);
                page3.SetActive(true);
                storyText.text = "With the power of music under Coda’s command, he warped the lands and its inhabitants into his dark image.";
                break;
            case 3:
                page3.SetActive(false);
                page4.SetActive(true);
                storyText.text = "Now Giocoso must travel his newly twisted home and defeat Coda’s minions in order to regain his lost power and restore Oratorio to its former glory.";
                break;
            case 4:
                SceneManager.LoadScene(sceneName: "Brandon-Test");
                break;
            default:
                break;
        }
        step++;
    }
}
