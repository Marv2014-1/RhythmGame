using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class BeatDetector : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;       // Reference to the AudioSource component
    public AudioClip audioClip;           // The audio clip to be played (This could be an array with multiple songs in the future)

    [Header("JSON Settings")]
    // The JSON file containing beat times should be placed in the Beat_Times subfolder within the Resources folder 
    public string subfolder = "Beat_Times"; // Subfolder within the Resources folder
    public string jsonFileName = "beat_times"; // JSON file name without the .json extension (This could be an array with multiple songs in the future)

    [Header("Timing Settings")]
    public float timingWindow = 0.15f;    // Allowed time window (in seconds) for a hit to be considered accurate
    public float delayBeforeStart = 2f;   // Delay before the music and detection start (in seconds)

    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText;  // UI text element to display feedback to the player

    [Header("Visual Settings")]
    public RectTransform beatVisualContainer; // Container for beat visual elements (UI)
    public GameObject beatVisualPrefab;       // Prefab for the beat visual (should have a RectTransform component)
    public float visualDuration = 5f;         // Duration (in seconds) that beat visuals are displayed before reaching the center

    private List<float> beatTimes = new List<float>(); // List to store the times of each beat
    private bool isPlaying = false;                     // Flag to indicate if the music is currently playing

    private float previousAudioTime = 0f;               // Song time in the previous frame

    // Dictionary to track the status of each beat:
    // 0 = unhandled, 1 = hit, -1 = missed
    private Dictionary<int, int> beatStatus = new Dictionary<int, int>();

    // List to manage active beat visuals currently on the screen
    private List<BeatVisual> activeBeatVisuals = new List<BeatVisual>();

    private double songStartTime; // DSP time when the song is scheduled to start

    // Event to notify subscribers when a beat is successfully hit
    public UnityEvent OnBeatHit;

    // Event to notify subscribers when a beat is missed
    public UnityEvent OnBeatMissed;

    /// <summary>
    /// Unity's Start method. Called before the first frame update.
    /// Initializes beat times and sets up the audio.
    /// </summary>
    void Start()
    {
        if (OnBeatHit == null)
            OnBeatHit = new UnityEvent();

        if (OnBeatMissed == null)
            OnBeatMissed = new UnityEvent();
        
        LoadBeatTimes(); // Load beat times from the JSON file
        SetupAudio();    // Set up the audio source and schedule playback
    }

    /// <summary>
    /// Unity's Update method. Called once per frame.
    /// Handles song playback, beat detection, and visual updates.
    /// </summary>
    void Update()
    {
        if (isPlaying)
        {
            double dspTime = AudioSettings.dspTime; // Current DSP time
            float songTime = (float)(dspTime - songStartTime); // Calculate song time based on DSP time

            float songLength = audioClip.length; // Total length of the song in seconds

            // If songTime exceeds song length, loop it
            if (songTime >= 0f)
            {
                songTime = songTime % songLength;
            }

            // Detect if the song has looped by checking if songTime has reset
            if (songTime < previousAudioTime)
            {
                OnSongLooped(); // Handle looping
            }

            // Check for any missed beats between the previous and current song times
            CheckMissedBeats(previousAudioTime, songTime);

            // Detect player input (space bar presses)
            DetectPlayerInput();

            // Update the positions of beat visuals on the screen
            UpdateBeatVisuals(songTime);

            // Update previousAudioTime for the next frame
            previousAudioTime = songTime;
        }
    }

    /// <summary>
    /// Method to handle actions when the song loops.
    /// Resets beat statuses and reinitializes beat visuals.
    /// </summary>
    void OnSongLooped()
    {
        // Reset all beat statuses to unhandled
        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // 0 indicates unhandled
        }

        // Destroy all active beat visuals on the screen
        foreach (BeatVisual beatVisual in activeBeatVisuals)
        {
            Destroy(beatVisual.rectTransform.gameObject);
        }
        activeBeatVisuals.Clear(); // Clear the list of active beat visuals

        // Re-initialize beat visuals for the new loop
        InitializeBeatVisuals();

        previousAudioTime = 0f; // Reset previousAudioTime

        Debug.Log("Song has looped. Variables have been reset.");
    }

    /// <summary>
    /// Loads beat times from a JSON file located in the specified Resources subfolder.
    /// Parses the JSON and initializes beat statuses and visuals.
    /// </summary>
    void LoadBeatTimes()
    {
        // Load the JSON file from Resources/Beat_Times
        TextAsset jsonFile = Resources.Load<TextAsset>(subfolder + "/" + jsonFileName);

        if (jsonFile != null)
        {
            Debug.Log("JSON file loaded successfully. Content: " + jsonFile.text);

            // Manually parse the JSON file content to extract beat times
            List<float> parsedBeatTimes = ManuallyParseBeatTimes(jsonFile.text);

            if (parsedBeatTimes != null && parsedBeatTimes.Count > 0)
            {
                // Ensure the beat times are sorted in ascending order
                beatTimes = parsedBeatTimes.OrderBy(bt => bt).ToList();

                // Initialize beat statuses as unhandled
                for (int i = 0; i < beatTimes.Count; i++)
                {
                    beatStatus[i] = 0; // 0 indicates unhandled
                }

                // Log each beat time for debugging purposes
                foreach (float beatTime in beatTimes)
                {
                    Debug.Log("Beat Time: " + beatTime);
                }

                InitializeBeatVisuals(); // Initialize visuals based on loaded beat times
            }
            else
            {
                Debug.LogError("Failed to parse the JSON content or no beats found.");
            }
        }
        else
        {
            Debug.LogError("Failed to load the JSON file from Resources/Beat_Times/" + jsonFileName);
        }
    }

    /// <summary>
    /// Manually parses a JSON string to extract a list of beat times.
    /// Assumes the JSON has a key "BeatTimes" with an array of float values.
    /// </summary>
    /// <param name="jsonText">The JSON content as a string.</param>
    /// <returns>A list of beat times as floats.</returns>
    List<float> ManuallyParseBeatTimes(string jsonText)
    {
        List<float> beatTimeList = new List<float>();

        // Step 1: Find the index of the "BeatTimes" key in the JSON
        string key = "\"BeatTimes\"";
        int keyIndex = jsonText.IndexOf(key);
        if (keyIndex == -1)
        {
            Debug.LogError("\"BeatTimes\" key not found in JSON.");
            return null;
        }

        // Step 2: Find the start of the array '[' after the key
        int arrayStart = jsonText.IndexOf('[', keyIndex);
        if (arrayStart == -1)
        {
            Debug.LogError("Start of array '[' not found after \"BeatTimes\" key.");
            return null;
        }

        // Step 3: Find the end of the array ']' after the start
        int arrayEnd = jsonText.IndexOf(']', arrayStart);
        if (arrayEnd == -1)
        {
            Debug.LogError("End of array ']' not found after \"BeatTimes\" key.");
            return null;
        }

        // Step 4: Extract the substring containing the numbers
        string arrayContent = jsonText.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
        // Debug.Log("Array Content: " + arrayContent);

        // Step 5: Split the string by commas to get individual beat time strings
        string[] numberStrings = arrayContent.Split(',');

        // Step 6: Convert each string to float and add to the list
        foreach (string numStr in numberStrings)
        {
            // Trim any whitespace and newline characters
            string trimmedStr = numStr.Trim();

            // Attempt to parse the string to float using invariant culture
            if (float.TryParse(trimmedStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float beatTime))
            {
                beatTimeList.Add(beatTime);
            }
            else
            {
                Debug.LogWarning("Failed to parse beat time: " + trimmedStr);
            }
        }

        return beatTimeList;
    }

    /// <summary>
    /// Sets up the AudioSource component by assigning the AudioClip and scheduling playback.
    /// </summary>
    void SetupAudio()
    {
        if (audioClip == null)
        {
            Debug.LogError("AudioClip not assigned!");
            return;
        }

        audioSource.clip = audioClip;   // Assign the audio clip to the AudioSource
        audioSource.loop = true;        // Enable looping of the audio clip
        StartCoroutine(StartAudioAfterDelay()); // Start the coroutine to play audio after a delay
    }

    /// <summary>
    /// Coroutine to start audio playback after a specified delay.
    /// Schedules the audio to start at a precise DSP time for synchronization.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    IEnumerator StartAudioAfterDelay()
    {
        // Calculate the DSP time when the song should start
        songStartTime = AudioSettings.dspTime + delayBeforeStart;

        // Schedule the AudioSource to play at the calculated DSP time
        audioSource.PlayScheduled(songStartTime);

        isPlaying = true; // Set the playing flag to true

        yield return null; // Wait for the next frame
    }

    /// <summary>
    /// Detects player input (space bar press) and checks if it aligns with a beat.
    /// </summary>
    void DetectPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Capture the precise song time when the key was pressed
            float inputTime = (float)(AudioSettings.dspTime - songStartTime);

            if (inputTime >= 0f)
            {
                // Handle looping by taking modulo with song length
                inputTime = inputTime % audioClip.length;
                CheckBeatAccuracy(inputTime); // Check if the input time aligns with a beat
            }
        }
    }

    /// <summary>
    /// Checks if the player's input time is within the timing window of any beat.
    /// Updates feedback and beat status accordingly.
    /// </summary>
    /// <param name="inputTime">The time (in seconds) when the player pressed the space bar.</param>
    void CheckBeatAccuracy(float inputTime)
    {
        // Use binary search to find the closest beat time to the input time
        int index = FindClosestIndex(inputTime);

        bool hit = false; // Flag to determine if a beat was hit

        // Check the beat time at index - 1
        if (index > 0 && Mathf.Abs(beatTimes[index - 1] - inputTime) <= timingWindow)
        {
            if (beatStatus[index - 1] == 0) // If the beat is unhandled
            {
                hit = true; // A beat has been hit
                MarkBeatAsHandled(index - 1, true); // Mark the beat as hit
            }
        }

        // If no hit yet, check the beat time at index
        if (!hit && index < beatTimes.Count && Mathf.Abs(beatTimes[index] - inputTime) <= timingWindow)
        {
            if (beatStatus[index] == 0) // If the beat is unhandled
            {
                hit = true; // A beat has been hit
                MarkBeatAsHandled(index, true); // Mark the beat as hit
            }
        }

        // Update feedback based on whether a beat was hit or missed
        if (hit)
        {
            feedbackText.text = "Perfect!";
            feedbackText.color = Color.green;
            Debug.Log("Beat hit at time: " + inputTime);

            // Invoke the OnBeatHit event
            OnBeatHit.Invoke();
        }
        else
        {
            feedbackText.text = "Miss!";
            feedbackText.color = Color.red;
            Debug.Log("Missed beat at time: " + inputTime);

            // Invoke the OnBeatMissed event
            OnBeatMissed.Invoke();
        }
    }

    /// <summary>
    /// Checks for any beats that were missed between the previous and current song times.
    /// Updates feedback and beat status for missed beats.
    /// </summary>
    /// <param name="previousTime">The song time in the previous frame.</param>
    /// <param name="currentTime">The current song time.</param>
    void CheckMissedBeats(float previousTime, float currentTime)
    {
        // Iterate through all beats to check if any were missed
        for (int i = 0; i < beatTimes.Count; i++)
        {
            float beatTime = beatTimes[i];

            // Handle looping by adjusting beatTime if necessary
            float adjustedBeatTime = beatTime;
            if (currentTime < previousTime)
            {
                // Loop occurred
                if (beatTime < previousTime)
                {
                    adjustedBeatTime += audioClip.length;
                }
            }

            // Skip beats that are still in the future relative to previousTime
            if (adjustedBeatTime + timingWindow < previousTime)
            {
                continue;
            }

            // Check if the beat time falls within the missed window
            if (adjustedBeatTime + timingWindow < currentTime)
            {
                if (beatStatus[i] == 0) // If the beat is unhandled
                {
                    feedbackText.text = "Beat missed!";
                    feedbackText.color = Color.red;
                    Debug.Log("Beat missed at time: " + beatTime);
                    MarkBeatAsHandled(i, false); // Mark the beat as missed
                }
            }
        }
    }

    /// <summary>
    /// Performs a binary search to find the index of the closest beat time to the input time.
    /// </summary>
    /// <param name="inputTime">The time to search for.</param>
    /// <returns>The index of the closest beat time.</returns>
    int FindClosestIndex(float inputTime)
    {
        int left = 0;
        int right = beatTimes.Count - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            float midTime = beatTimes[mid];

            if (midTime < inputTime)
            {
                left = mid + 1;
            }
            else if (midTime > inputTime)
            {
                right = mid - 1;
            }
            else
            {
                // Exact match found
                return mid;
            }
        }

        // If no exact match, return the insertion point
        return left;
    }

    /// <summary>
    /// Marks a beat as handled (hit or missed) in the beatStatus dictionary.
    /// </summary>
    /// <param name="index">The index of the beat in the beatTimes list.</param>
    /// <param name="isHit">True if the beat was hit, false if missed.</param>
    void MarkBeatAsHandled(int index, bool isHit)
    {
        beatStatus[index] = isHit ? 1 : -1; // 1 for hit, -1 for missed
    }

    /// <summary>
    /// Initializes beat visuals for beats that are within the visual duration from the current song time.
    /// </summary>
    void InitializeBeatVisuals()
    {
        // Calculate the current song time based on DSP time
        float songTime = (float)(AudioSettings.dspTime - songStartTime);

        // Iterate through all beat times to create visuals for upcoming beats
        foreach (float beatTime in beatTimes)
        {
            // Only create visuals for beats that are within the next visualDuration seconds
            if (beatTime - songTime >= 0f && beatTime - songTime <= visualDuration)
            {
                CreateBeatVisual(beatTime); // Instantiate beat visuals for the beat time
            }
        }
    }

    /// <summary>
    /// Creates visual representations of a beat on both the left and right sides of the screen.
    /// Positions them at the bottom dynamically based on screen size.
    /// </summary>
    /// <param name="beatTime">The time (in seconds) when the beat occurs.</param>
    void CreateBeatVisual(float beatTime)
    {
        // Desired size
        Vector2 desiredSize = new Vector2(30f, 100f);

        // Create left beat visual
        GameObject leftBeatVisualObject = Instantiate(beatVisualPrefab, beatVisualContainer);
        RectTransform leftRectTransform = leftBeatVisualObject.GetComponent<RectTransform>();

        // Set anchors and pivot to bottom-left
        leftRectTransform.anchorMin = new Vector2(0f, 0f); // Left edge, bottom
        leftRectTransform.anchorMax = new Vector2(0f, 0f);
        leftRectTransform.pivot = new Vector2(0f, 0f);

        // Set size
        leftRectTransform.sizeDelta = desiredSize;

        // Position at bottom-left corner
        leftRectTransform.anchoredPosition = new Vector2(0f, 0f);
        BeatVisual leftBeatVisual = new BeatVisual(leftRectTransform, beatTime, true);
        activeBeatVisuals.Add(leftBeatVisual);

        // Create right beat visual
        GameObject rightBeatVisualObject = Instantiate(beatVisualPrefab, beatVisualContainer);
        RectTransform rightRectTransform = rightBeatVisualObject.GetComponent<RectTransform>();

        // Set anchors and pivot to bottom-right
        rightRectTransform.anchorMin = new Vector2(1f, 0f); // Right edge, bottom
        rightRectTransform.anchorMax = new Vector2(1f, 0f);
        rightRectTransform.pivot = new Vector2(1f, 0f);

        // Set size
        rightRectTransform.sizeDelta = desiredSize;

        // Position at bottom-right corner
        rightRectTransform.anchoredPosition = new Vector2(0f, 0f);
        BeatVisual rightBeatVisual = new BeatVisual(rightRectTransform, beatTime, false);
        activeBeatVisuals.Add(rightBeatVisual);

        leftRectTransform.anchoredPosition = new Vector2(0f, 25f);
        rightRectTransform.anchoredPosition = new Vector2(0f, 25f);

    }

    /// <summary>
    /// Updates the positions of all active beat visuals based on the current song time.
    /// </summary>
    /// <param name="songTime">The current time of the song (in seconds).</param>
    void UpdateBeatVisuals(float songTime)
    {
        var visualsToRemove = new List<BeatVisual>();

        foreach (BeatVisual beatVisual in activeBeatVisuals)
        {
            float timeUntilBeat = beatVisual.beatTime - songTime;

            // Handle looping
            if (timeUntilBeat < -audioClip.length / 2)
            {
                timeUntilBeat += audioClip.length;
            }
            else if (timeUntilBeat > audioClip.length / 2)
            {
                timeUntilBeat -= audioClip.length;
            }

            float normalizedTime = 1 - (timeUntilBeat / visualDuration);

            if (normalizedTime >= 1)
            {
                Destroy(beatVisual.rectTransform.gameObject);
                visualsToRemove.Add(beatVisual);
            }
            else
            {
                // Calculate the target X position
                float halfWidth = beatVisualContainer.rect.width / 2;

                // Determine target X based on direction
                float targetX = beatVisual.fromLeft ? halfWidth : -halfWidth;

                // Move from the edge (x=0) to the center over time
                float xPosition = Mathf.Lerp(0f, targetX, normalizedTime);

                // Update the anchored position without altering the size
                beatVisual.rectTransform.anchoredPosition = new Vector2(xPosition, beatVisual.rectTransform.anchoredPosition.y);
            }
        }

        foreach (BeatVisual beatVisual in visualsToRemove)
        {
            activeBeatVisuals.Remove(beatVisual);
        }

        // Add new visuals for upcoming beats
        foreach (float beatTime in beatTimes)
        {
            float timeUntilBeat = beatTime - songTime;

            // Handle looping
            if (timeUntilBeat < -audioClip.length / 2)
            {
                timeUntilBeat += audioClip.length;
            }
            else if (timeUntilBeat > audioClip.length / 2)
            {
                timeUntilBeat -= audioClip.length;
            }

            if (timeUntilBeat >= 0f && timeUntilBeat <= visualDuration)
            {
                // Check if visuals from both sides exist
                bool leftVisualExists = activeBeatVisuals.Exists(bv => bv.beatTime == beatTime && bv.fromLeft == true);
                bool rightVisualExists = activeBeatVisuals.Exists(bv => bv.beatTime == beatTime && bv.fromLeft == false);

                if (!leftVisualExists)
                {
                    CreateBeatVisual(beatTime);
                }
                // Optionally, if you want separate visuals for both sides, ensure both are created
                if (!rightVisualExists)
                {
                    CreateBeatVisual(beatTime);
                }
            }
        }
    }



    /// <summary>
    /// Inner class to represent a visual element for a beat.
    /// Stores the RectTransform, beat time, and origin side (left or right).
    /// </summary>
    class BeatVisual
    {
        public RectTransform rectTransform; // Reference to the RectTransform component of the visual
        public float beatTime;              // The time (in seconds) when this beat occurs
        public bool fromLeft;               // Indicates if the visual comes from the left (true) or right (false)

        /// <summary>
        /// Constructor to initialize a BeatVisual instance.
        /// </summary>
        /// <param name="rectTransform">The RectTransform of the visual.</param>
        /// <param name="beatTime">The beat time associated with this visual.</param>
        /// <param name="fromLeft">True if the visual comes from the left, false if from the right.</param>
        public BeatVisual(RectTransform rectTransform, float beatTime, bool fromLeft)
        {
            this.rectTransform = rectTransform;
            this.beatTime = beatTime;
            this.fromLeft = fromLeft;
        }
    }
}
