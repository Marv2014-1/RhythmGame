using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BeatDetector : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("JSON Settings")]
    public string subfolder = "Beat_Times";
    public string jsonFileName = "";

    [Header("Timing Settings")]
    public float timingWindow = 0.15f;

    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Visual Settings")]
    public RectTransform beatVisualContainer;
    public GameObject beatVisualPrefab;
    public float visualDuration = 5f;

    private List<float> beatTimes = new List<float>();
    private Dictionary<int, int> beatStatus = new Dictionary<int, int>();
    private float previousAudioTime = 0f;

    private BeatTimeLoader beatTimeLoader;
    private AudioManager audioManager;
    private BeatVisualManager beatVisualManager;

    public UnityEvent OnBeatHit;
    public UnityEvent OnBeatMissed;
    public UnityEvent OnBeatOccurred;

    private float songStartTime;
    private bool songStarted = false;
    private float songStartDelay = 5f;

    [Header("Playlist Settings")]
    public List<Song> playlist = new List<Song>();

    private int currentSongIndex = -1;
    private int currentLoopCount = 0;

    private bool first = true;
    void Awake()
    {
        beatTimeLoader = new BeatTimeLoader();
        audioManager = gameObject.AddComponent<AudioManager>();
        beatVisualManager = gameObject.AddComponent<BeatVisualManager>();

        audioManager.audioSource = audioSource;

        beatVisualManager.beatVisualContainer = beatVisualContainer;
        beatVisualManager.beatVisualPrefab = beatVisualPrefab;
        beatVisualManager.visualDuration = visualDuration;

        beatVisualManager.SetAudioManager(audioManager);
    }

    void Start()
    {

        if (OnBeatHit == null)
            OnBeatHit = new UnityEvent();

        if (OnBeatMissed == null)
            OnBeatMissed = new UnityEvent();

        if (OnBeatOccurred == null)
            OnBeatOccurred = new UnityEvent();

        if (playlist.Count > 0)
        {
            currentSongIndex = -1;
            currentLoopCount = 0;
            // Set songStartTime
            songStartTime = Time.time + songStartDelay;
            songStarted = false;

            // Start the coroutine to start the song after delay
            StartCoroutine(TransitionToNextSong());

        }
        else
        {
            Debug.LogError("Playlist is empty!");
        }
    }

    void Update()
    {
        float songTime = songStarted ? audioManager.GetSongTime() : Time.time - songStartTime;
        beatVisualManager.UpdateBeatVisuals(beatTimes, songTime);

        if (songStarted)
        {
            // Detect if the song has looped back to the beginning
            if (songTime < previousAudioTime)
            {
                currentLoopCount++;
                if (currentLoopCount < playlist[currentSongIndex].loopCount)
                {
                    // Replay the current song
                    audioManager.PlayAudio();
                    OnSongLooped();
                }
                else
                {
                    // Transition to the next song
                    StartCoroutine(TransitionToNextSong());
                }
            }

            CheckMissedBeats(previousAudioTime, songTime);
            DetectPlayerInput(songTime);
            beatVisualManager.UpdateBeatVisuals(beatTimes, songTime);

            previousAudioTime = songTime;
        }
    }

    private IEnumerator TransitionToNextSong()
    {
        audioManager.StopAudio();

        if (currentSongIndex >= playlist.Count)
        {
            Debug.Log("Playlist ended.");
            songStarted = false;
            beatVisualManager.ClearAllBeatVisuals();
            yield break;
        }

        currentLoopCount = 0;
        currentSongIndex++;
        LoadBeatTimes(); // Load the next song's beat times
        audioManager.SetupAudio(playlist[currentSongIndex]);
        audioManager.StopAudio();
        Debug.Log($"Transitioning to next song: {playlist[currentSongIndex].audioClip.name}");

        yield return new WaitForSeconds(5f); // 5-second delay

        audioManager.PlayAudio();
        songStarted = true;
    }

    /// <summary>
    /// Loads beat times using BeatTimeLoader and initializes beat statuses and visuals.
    /// </summary>
    private void LoadBeatTimes()
    {
        string jsonFileName = playlist[currentSongIndex].jsonFileName;
        beatTimes = beatTimeLoader.LoadBeatTimes(subfolder, jsonFileName);

        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // 0 = unhandled
            Debug.Log($"Beat Time: {beatTimes[i]}");
        }

        // Initialize beat visuals with songTime = 0f
        beatVisualManager.InitializeBeatVisuals(beatTimes, 0f);
    }


    /// <summary>
    /// Handles actions when the song loops.
    /// </summary>
    private void OnSongLooped()
    {
        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // Reset all beat statuses to unhandled
        }

        audioManager.audioSource.time = 0f; // Reset audio time
        previousAudioTime = 0f;
        beatVisualManager.InitializeBeatVisuals(beatTimes, 0f);
        Debug.Log("Song has looped. Variables have been reset.");
    }

    /// <summary>
    /// Detects player input (space bar press) and checks beat accuracy.
    /// </summary>
    /// <param name="songTime">Current song time.</param>
    private void DetectPlayerInput(float songTime)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float inputTime = songTime;
            CheckBeatAccuracy(inputTime);
        }
    }

    /// <summary>
    /// Checks if the player's input is within the timing window of any beat.
    /// </summary>
    /// <param name="inputTime">The time when the player pressed the space bar.</param>
    private void CheckBeatAccuracy(float inputTime)
    {
        int index = FindClosestIndex(inputTime);
        bool hit = false;

        if (index > 0 && Mathf.Abs(beatTimes[index - 1] - inputTime) <= timingWindow)
        {
            if (beatStatus[index - 1] == 0)
            {
                hit = true;
                MarkBeatAsHandled(index - 1, true);
            }
        }

        if (!hit && index < beatTimes.Count && Mathf.Abs(beatTimes[index] - inputTime) <= timingWindow)
        {
            if (beatStatus[index] == 0)
            {
                hit = true;
                MarkBeatAsHandled(index, true);
            }
        }

        if (hit)
        {
            feedbackText.text = "Perfect!";
            feedbackText.color = Color.green;
            Debug.Log($"Beat hit at time: {inputTime}");
            OnBeatHit.Invoke();
            OnBeatOccurred.Invoke();
        }
        else
        {
            feedbackText.text = "Miss!";
            feedbackText.color = Color.red;
            Debug.Log($"Missed beat at time: {inputTime}");
            OnBeatMissed.Invoke();
            OnBeatOccurred.Invoke();
        }
    }

    /// <summary>
    /// Checks for any beats that were missed between the previous and current song times.
    /// </summary>
    /// <param name="previousTime">Previous song time.</param>
    /// <param name="currentTime">Current song time.</param>
    private void CheckMissedBeats(float previousTime, float currentTime)
    {
        float audioClipLength = audioManager.audioClip.length;

        for (int i = 0; i < beatTimes.Count; i++)
        {
            float beatTime = beatTimes[i];
            float adjustedBeatTime = beatTime;

            if (currentTime < previousTime)
            {
                // Loop occurred
                if (beatTime < previousTime)
                {
                    adjustedBeatTime += audioClipLength;
                }
            }

            if (adjustedBeatTime + timingWindow < previousTime)
            {
                continue;
            }

            if (adjustedBeatTime + timingWindow < currentTime)
            {
                if (beatStatus[i] == 0)
                {
                    feedbackText.text = "Beat missed!";
                    feedbackText.color = Color.red;
                    Debug.Log($"Beat missed at time: {beatTime}");
                    MarkBeatAsHandled(i, false);
                    OnBeatMissed.Invoke();
                    OnBeatOccurred.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Finds the closest index of a beat time relative to the input time using binary search.
    /// </summary>
    /// <param name="inputTime">The time to search for.</param>
    /// <returns>The index of the closest beat time.</returns>
    private int FindClosestIndex(float inputTime)
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
    /// Marks a beat as handled (hit or missed).
    /// </summary>
    /// <param name="index">Index of the beat in the beatTimes list.</param>
    /// <param name="isHit">True if the beat was hit, false if missed.</param>
    private void MarkBeatAsHandled(int index, bool isHit)
    {
        beatStatus[index] = isHit ? 1 : -1;
    }
}
