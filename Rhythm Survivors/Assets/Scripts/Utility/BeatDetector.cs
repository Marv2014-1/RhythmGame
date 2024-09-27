using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class BeatDetector : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("JSON Settings")]
    // The JSON file should be placed in the Beat_Times subfolder within the Resources folder 
    public string subfolder = "Beat_Times"; // Subfolder within Resources folder
    public string jsonFileName = "beat_times"; // Without .json extension

    [Header("Timing Settings")]
    public float timingWindow = 0.15f; // +/- 0.15 seconds
    public float delayBeforeStart = 1f; // Delay before music and detection start

    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText;

    private List<float> beatTimes = new List<float>();
    private bool isPlaying = false;

    private int nextBeatIndex = 0; // Index of the next upcoming beat
    private float previousAudioTime = 0f; // Time in the previous frame

    // Keep track of handled beats: 0 = unhandled, 1 = hit, -1 = missed
    private Dictionary<int, int> beatStatus = new Dictionary<int, int>();

    void Start()
    {
        LoadBeatTimes();
        SetupAudio();
    }

    void Update()
    {
        if (isPlaying)
        {
            float currentAudioTime = audioSource.time;

            // Detect if audio has looped
            if (currentAudioTime < previousAudioTime)
            {
                // Audio has looped
                OnAudioLooped();
            }

            // Check for missed beats
            CheckMissedBeats(currentAudioTime);

            DetectPlayerInput(currentAudioTime);

            previousAudioTime = currentAudioTime;
        }
    }

    void OnAudioLooped()
    {
        // Reset variables
        nextBeatIndex = 0;

        // Reset beatStatus
        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // Reset to unhandled
        }

        previousAudioTime = 0f;

        Debug.Log("Audio has looped. Variables have been reset.");
    }

    void LoadBeatTimes()
    {
        // Load the JSON file from Resources/Beat_Times
        TextAsset jsonFile = Resources.Load<TextAsset>(subfolder + "/" + jsonFileName);

        if (jsonFile != null)
        {
            Debug.Log("JSON file loaded successfully. Content: " + jsonFile.text);

            // Manually parse the JSON file content
            List<float> parsedBeatTimes = ManuallyParseBeatTimes(jsonFile.text);

            if (parsedBeatTimes != null && parsedBeatTimes.Count > 0)
            {
                // Ensure the beat times are sorted
                beatTimes = parsedBeatTimes.OrderBy(bt => bt).ToList();

                // Initialize beat statuses
                for (int i = 0; i < beatTimes.Count; i++)
                {
                    beatStatus[i] = 0; // 0 indicates unhandled
                }

                foreach (float beatTime in beatTimes)
                {
                    Debug.Log("Beat Time: " + beatTime);
                }
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
    /// Manually parses the JSON string to extract beat times.
    /// </summary>
    /// <param name="jsonText">The JSON content as a string.</param>
    /// <returns>A list of beat times as floats.</returns>
    List<float> ManuallyParseBeatTimes(string jsonText)
    {
        List<float> beatTimeList = new List<float>();

        // Step 1: Find the index of "BeatTimes"
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

        // Step 5: Split the string by commas
        string[] numberStrings = arrayContent.Split(',');

        // Step 6: Convert each string to float and add to the list
        foreach (string numStr in numberStrings)
        {
            // Trim any whitespace and newline characters
            string trimmedStr = numStr.Trim();

            // Attempt to parse the string to float
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

    void SetupAudio()
    {
        if (audioClip == null)
        {
            Debug.LogError("AudioClip not assigned!");
            return;
        }

        audioSource.clip = audioClip;
        audioSource.loop = true; // Enable looping
        StartCoroutine(StartAudioAfterDelay());
    }

    IEnumerator StartAudioAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        audioSource.Play();
        isPlaying = true;
    }

    void DetectPlayerInput(float currentTime)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckBeatAccuracy(currentTime);
        }
    }

    void CheckBeatAccuracy(float inputTime)
    {
        // Use binary search to find the closest beat time
        int index = FindClosestIndex(inputTime);

        bool hit = false;

        // Check the beat time at index - 1
        if (index > 0 && Mathf.Abs(beatTimes[index - 1] - inputTime) <= timingWindow)
        {
            if (beatStatus[index - 1] == 0) // If unhandled
            {
                hit = true;
                MarkBeatAsHandled(index - 1, true);
            }
        }

        // Check the beat time at index
        if (!hit && index < beatTimes.Count && Mathf.Abs(beatTimes[index] - inputTime) <= timingWindow)
        {
            if (beatStatus[index] == 0) // If unhandled
            {
                hit = true;
                MarkBeatAsHandled(index, true);
            }
        }

        if (hit)
        {
            feedbackText.text = "Perfect!";
            feedbackText.color = Color.green;
            Debug.Log("Beat hit at time: " + inputTime);
        }
        else
        {
            feedbackText.text = "Miss!";
            feedbackText.color = Color.red;
            Debug.Log("Missed beat at time: " + inputTime);
        }
    }

    void CheckMissedBeats(float currentTime)
    {
        while (nextBeatIndex < beatTimes.Count && beatTimes[nextBeatIndex] + timingWindow <= currentTime)
        {
            if (beatStatus[nextBeatIndex] == 0) // If unhandled
            {
                feedbackText.text = "Beat missed!";
                feedbackText.color = Color.red;
                Debug.Log("Beat missed at time: " + beatTimes[nextBeatIndex]);
                MarkBeatAsHandled(nextBeatIndex, false);
            }
            nextBeatIndex++;
        }
    }

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
                // Exact match
                return mid;
            }
        }

        // left is the insertion point
        return left;
    }

    void MarkBeatAsHandled(int index, bool isHit)
    {
        beatStatus[index] = isHit ? 1 : -1; // 1 for hit, -1 for missed
    }
}
