using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBeatIndicatorManager : MonoBehaviour
{
    public GameObject circlePrefab; // Prefab for the circle UI element
    public int requiredBeats = 3;   // Number of circles to display
    private float xInit = -15f;     // Initial x position for circles
    private float yInit = 0f;       // Initial y position for circles
    private float xSpacing = 13f;   // Spacing between circles horizontally
    private float ySpacing = 11f;   // Spacing between circles vertically
    private int perRow = 3;         // How many circles before starting new row

    private List<Image> circles = new List<Image>(); // List to hold circle images
    private int currentBeatCount = 0;                // Current count of successful beats

    // Reference to the BeatDetector to subscribe to its events
    public BeatDetector beatDetector;

    // Reference to the active coroutine to prevent multiple instances
    private Coroutine beatCoroutine;

    void Start()
    {
        GenerateCircles();

        // Subscribe to BeatDetector events
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.AddListener(OnBeatHit);
            // beatDetector.OnBeatMissed.AddListener(OnBeatMissed);
        }
        else
        {
            Debug.LogError("BeatDetector reference is not set in BeatIndicatorManager.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (beatDetector != null)
        {
            beatDetector.OnBeatHit.RemoveListener(OnBeatHit);
            // beatDetector.OnBeatMissed.RemoveListener(OnBeatMissed);
        }
    }

    // Generates the UI circles based on requiredBeats
    void GenerateCircles()
    {
        // Clear existing circles
        foreach (Image circle in circles)
        {
            Destroy(circle.gameObject);
        }
        circles.Clear();

        if (requiredBeats <= 0)
        {
            Debug.LogWarning("Required beats must be greater than zero.");
            return;
        }

        for (int i = 0; i < requiredBeats; i++)
        {
            GameObject circleObj = Instantiate(circlePrefab, transform);
            RectTransform rectTransform = circleObj.GetComponent<RectTransform>();

            // Calculate the position based number of beats
            float x = xInit + ((i % perRow) * xSpacing);
            float y = yInit;
            y += Mathf.Floor((requiredBeats - 1) / perRow) * (ySpacing / 2);
            if (i >= perRow)
            {
                y += Mathf.Floor(i / perRow) * -ySpacing;
            }
            rectTransform.anchoredPosition = new Vector2(x, y);

            Image circleImage = circleObj.GetComponent<Image>();
            circleImage.color = Color.grey; // Initial inactive color
            circles.Add(circleImage);
        }

        // Reset the current beat count
        currentBeatCount = 0;
        UpdateBeatCount(currentBeatCount);
    }

    // Event handler for OnBeatHit
    void OnBeatHit()
    {
        // Increment the current beat count
        currentBeatCount++;
        if (currentBeatCount >= requiredBeats)
        {
            currentBeatCount = 0;
            // Start the coroutine to activate all circles temporarily
            beatCoroutine = StartCoroutine(ActivateAllCirclesTemporarily(.1f));
        }
        else
        {
            // Update the beat count
            UpdateBeatCount(currentBeatCount);
        }
    }

    // Coroutine to activate all circles, wait, and then reset
    IEnumerator ActivateAllCirclesTemporarily(float delay)
    {
        // Activate all circles
        foreach (var circle in circles)
        {
            circle.color = Color.green; // Active beat color
        }

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Reset the circles based on the current beat count
        UpdateBeatCount(currentBeatCount);

        // Clear the coroutine reference
        beatCoroutine = null;
    }

    // Event handler for OnBeatMissed
    // void OnBeatMissed()
    // {
    //     // Reset the current beat count or handle as needed
    //     if (currentBeatCount > 0)
    //     {
    //         currentBeatCount -= 1;
    //     }
    //     UpdateBeatCount(currentBeatCount);
    // }

    // Updates the circles based on the current beat count
    public void UpdateBeatCount(int beatCount)
    {
        for (int i = 0; i < circles.Count; i++)
        {
            if (i < beatCount)
            {
                circles[i].color = Color.green; // Active beat
            }
            else
            {
                circles[i].color = Color.black;  // Inactive beat
            }
        }
    }

    // Allows dynamic adjustment of required beats
    public void SetRequiredBeats(int beats)
    {
        requiredBeats = beats;
        GenerateCircles();
    }
}
