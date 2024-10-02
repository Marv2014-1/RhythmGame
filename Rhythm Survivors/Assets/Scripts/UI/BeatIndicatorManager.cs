using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatIndicatorManager : MonoBehaviour
{
    
    public GameObject circlePrefab; // Prefab for the circle UI element
    public int requiredBeats = 3; // Number of circles to display
    public float spacing = 30f; // Spacing between circles
    private List<Image> circles = new List<Image>(); // List to hold circle images

    [Header("Arc Settings")]
    public float arcRadius = 100f;      // Distance from the pivot to each circle
    public float totalArcAngle = 180f;  // Total angle of the arc in degrees
    public float startAngle = 90f;       // Starting angle of the arc in degrees

     [Header("Camera Settings")]
    public Camera mainCamera; // Assign your main camera in the Inspector
    
    [Header("Desired Screen Size")]
    [Tooltip("Desired radius of the circle in pixels.")]
    public float desiredScreenRadius = 50f;

    private SpriteRenderer spriteRenderer;


    void Start()
    {
        GenerateCircles();
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

        // Calculate the angle between each circle
        float angleStep = requiredBeats > 1 ? totalArcAngle / (requiredBeats - 1) : 0f;

        for (int i = 0; i < requiredBeats; i++)
        {
            GameObject circleObj = Instantiate(circlePrefab, transform);
            RectTransform rectTransform = circleObj.GetComponent<RectTransform>();

            // Calculate the angle for this circle
            float currentAngle = startAngle - (totalArcAngle / 2) + (angleStep * i);

            // Convert angle to radians
            float angleRad = currentAngle * Mathf.Deg2Rad;

            // Calculate the position based on the angle and radius
            float x = arcRadius * Mathf.Cos(angleRad);
            float y = arcRadius * Mathf.Sin(angleRad);
            rectTransform.anchoredPosition = new Vector2(x, y);

            Image circleImage = circleObj.GetComponent<Image>();
            circleImage.color = Color.grey; // Initial inactive color
            circles.Add(circleImage);
        }
    }
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
                circles[i].color = Color.grey; // Inactive beat
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

