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
    // Allows the player to hit beats that have been destroyed
    private Dictionary<int, int> beatStatus = new Dictionary<int, int>();
    private float previousAudioTime = 0f;

    private BeatTimeLoader beatTimeLoader;
    private AudioManager audioManager;
    private BeatVisualManager beatVisualManager;

    public UnityEvent OnBeatHit;
    public UnityEvent OnBeatMissed;
    public UnityEvent OnBeatOccurred;
    private Coroutine clearFeedbackCoroutine;

    private bool songStarted = false;

    [Header("Playlist Settings")]
    public List<Song> playlist = new List<Song>();

    private int currentSongIndex = -1;
    private int currentLoopCount = 0;

    // private List<List<float>> allBeatTimes = new List<List<float>>();
    private List<List<float>> allBeatTimesWDelay = new List<List<float>>();
    public float delay = 4.0f;
    private float songStartTime = 0f;

    private bool isTransitionSong = false;
    private bool isLoopingSong = false;
    private bool canCheck = false;
    private bool canCheckMissedBeats = false;

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
        // Initialize UnityEvents if they are null
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
            songStarted = false;

            // Preload all beat times and audio clips
            for (int i = 0; i < playlist.Count; i++)
            {
                string jsonFileName = playlist[i].jsonFileName;
                // List<float> songBeats = beatTimeLoader.LoadBeatTimes(subfolder, jsonFileName, 0);
                // allBeatTimes.Add(songBeats);
                List<float> songBeatsWDelay = beatTimeLoader.LoadBeatTimes(subfolder, jsonFileName, delay - playlist[i].delayOffset);
                allBeatTimesWDelay.Add(songBeatsWDelay);

                // Preload audio clips
                if (playlist[i].audioClip != null)
                {
                    if (!playlist[i].audioClip.preloadAudioData)
                    {
                        playlist[i].audioClip.LoadAudioData();
                    }
                }
                else
                {
                    Debug.LogError($"AudioClip for song {i} is null!");
                }
            }

            // Start the coroutine to transition to the next song without delay
            StartCoroutine(TransitionToNextSong());
        }
        else
        {
            Debug.LogError("Playlist is empty!");
        }
    }

    void Update()
    {
        if (!songStarted)
            return;

        // Calculate songTime for beat detection and visuals
        float songTime = Time.time - songStartTime;
        float adjustedSongTime = songTime - delay;
        float adjustedPreviousTime = previousAudioTime - delay;

        beatVisualManager.UpdateBeatVisuals(beatTimes, adjustedSongTime);

        // Get the current audio time for loop detection
        float audioSongTime = audioManager.GetSongTime();

        // Detect if the audio has looped back to the beginning
        if (audioSongTime < adjustedPreviousTime)
        {
            currentLoopCount++;
            if (currentLoopCount <= playlist[currentSongIndex].loopCount)
            {
                // Replay the current song
                // StartCoroutine(OnSongLooped());
            }
            else
            {
                // Transition to the next song
                StartCoroutine(TransitionToNextSong());
            }
        }

        CheckMissedBeats(adjustedPreviousTime, adjustedSongTime);
        DetectPlayerInput(adjustedSongTime);

        // Update previousAudioTime with the current audio time
        previousAudioTime = audioSongTime;
    }

    private IEnumerator TransitionToNextSong()
    {
        canCheck = false;
        canCheckMissedBeats = false;

        currentSongIndex++;
        Debug.Log("Current Song Index: " + currentSongIndex);
        Debug.Log("Current playlist size: " + playlist.Count);
        if (currentSongIndex >= playlist.Count)
        {
            Debug.Log("Playlist ended.");
            songStarted = false;
            beatVisualManager.ClearAllBeatVisuals();

            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.EndRun();
            }
            else
            {
                Debug.LogWarning("PauseMenu not found in the scene.");
            }

            yield break;
        }

        currentLoopCount = 0;
        isTransitionSong = true;
        isLoopingSong = false;

        // Use delayed beat times when transitioning to the next song
        beatTimes = allBeatTimesWDelay[currentSongIndex];
        delay = delay - playlist[currentSongIndex].delayOffset;

        // Initialize beat statuses
        beatStatus.Clear();
        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // 0 = unhandled
        }

        // Initialize beat visuals with songTime = 0f
        beatVisualManager.InitializeBeatVisuals(beatTimes, 0f);

        audioManager.SetupAudio(playlist[currentSongIndex]);
        Debug.Log($"Transitioning to next song: {playlist[currentSongIndex].audioClip.name}");

        songStartTime = Time.time - delay;

        songStarted = true;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay - 1);

        canCheck = true;

        yield return new WaitForSeconds(1);

        canCheckMissedBeats = true;


        // Reset previousAudioTime
        previousAudioTime = 0f;

        // Start the audio
        audioManager.PlayAudio();
    }

    // private IEnumerator OnSongLooped()
    // {
    //     isTransitionSong = false;
    //     isLoopingSong = true;

    //     // Use delayed beat times when the song loops
    //     beatTimes = allBeatTimes[currentSongIndex];

    //     // Reset beat statuses
    //     beatStatus.Clear();
    //     for (int i = 0; i < beatTimes.Count; i++)
    //     {
    //         beatStatus[i] = 0; // Reset all beat statuses to unhandled
    //     }

    //     // Reset songStartTime and previousAudioTime
    //     songStartTime = Time.time;
    //     previousAudioTime = 0f;

    //     // Re-initialize beat visuals with songTime = 0f
    //     beatVisualManager.InitializeBeatVisuals(beatTimes, 0f);

    //     // Wait for the specified delay
    //     yield return new WaitForSeconds(1);

    //     // Ensure audio starts from the beginning
    //     audioManager.audioSource.time = 0f;

    //     // Start the audio
    //     audioManager.PlayAudio();

    //     songStarted = true;
    // }

    private void DetectPlayerInput(float adjustedSongTime)
    {
        if (Input.GetKeyDown(KeyCode.Space) && canCheck)
        {
            CheckBeatAccuracy(adjustedSongTime);
        }
    }

    public void CheckBeatAccuracy(float inputTime)
    {
        Debug.Log($"--- Checking Beat Accuracy ---");
        Debug.Log($"Input Time: {inputTime}");

        int index = FindClosestIndex(inputTime);
        Debug.Log($"Initial Closest Index: {index}");

        bool hit = false;
        float difference = float.MaxValue;
        int beatIndex = -1;

        // Check the previous beat
        if (index > 0)
        {
            float prevDifference = Mathf.Abs(beatTimes[index - 1] - inputTime);
            Debug.Log($"[Previous Beat] Index: {index - 1}, Beat Time: {beatTimes[index - 1]}, Difference: {prevDifference}, Status: {beatStatus[index - 1]}");
            if (prevDifference <= timingWindow && beatStatus[index - 1] == 0)
            {
                difference = prevDifference;
                beatIndex = index - 1;
                hit = true;
                Debug.Log($"--> [Previous Beat Hit] Difference: {difference}");
            }
            else
            {
                Debug.Log($"--> [Previous Beat Missed] Difference: {difference}");
            }
        }
        else
        {
            Debug.Log($"[Previous Beat] Skipped (index <= 0)");
        }

        // Check the current beat
        if (!hit && index < beatTimes.Count)
        {
            float currentDifference = Mathf.Abs(beatTimes[index] - inputTime);
            Debug.Log($"[Current Beat] Index: {index}, Beat Time: {beatTimes[index]}, Difference: {currentDifference}, Status: {beatStatus[index]}");
            if (currentDifference <= timingWindow && beatStatus[index] == 0)
            {
                difference = currentDifference;
                beatIndex = index;
                hit = true;
                Debug.Log($"--> [Current Beat Hit] Difference: {difference}");
            }
            else
            {
                Debug.Log($"--> [Current Beat Missed] Difference: {difference}");
            }
        }
        else
        {
            Debug.Log($"[Current Beat] Skipped (Already hit or index out of range)");
        }

        if (hit && beatIndex != -1)
        {
            MarkBeatAsHandled(beatIndex, true);

            if (difference <= timingWindow / 4f)
            {
                // Perfect hit
                SetFeedback("Perfect!", Color.green);

                // Invoke OnBeatHit twice
                OnBeatHit.Invoke();
                OnBeatHit.Invoke();

                OnBeatOccurred.Invoke();

                Debug.Log($"[Result] Perfect Hit! Beat Index: {beatIndex}, Difference: {difference}");
            }
            else
            {
                // Nice hit
                SetFeedback("Nice!", Color.yellow);

                OnBeatHit.Invoke();

                OnBeatOccurred.Invoke();

                Debug.Log($"[Result] Nice Hit! Beat Index: {beatIndex}, Difference: {difference}");
            }

            // Optional: Log the hit details
            Debug.Log($"[Hit Details] Input Time: {inputTime}, Closest Beat Time: {beatTimes[beatIndex]}, Difference: {difference}");
        }
        else
        {
            // Missed the beat
            SetFeedback("Miss!", Color.red);

            OnBeatMissed.Invoke();
            OnBeatOccurred.Invoke();

            // Optional: Log the miss details
            string closestBeatTime = (index < beatTimes.Count) ? beatTimes[index].ToString() : "N/A";
            Debug.Log($"[Result] Missed Beat! Input Time: {inputTime}, Closest Beat Time: {closestBeatTime}");
        }
        // print all beat status
        for (int i = 0; i < beatTimes.Count; i++)
        {
            Debug.Log($"Beat {i} Status: {beatStatus[i]}");
        }

        Debug.Log($"--- End of Check ---\n");
    }

    private void SetFeedback(string message, Color color)
    {
        // Set the feedback text and color
        feedbackText.text = message;
        feedbackText.color = color;

        // If a coroutine is already running, stop it
        if (clearFeedbackCoroutine != null)
        {
            StopCoroutine(clearFeedbackCoroutine);
        }

        // Start the coroutine to clear the feedback text after 0.15 seconds
        clearFeedbackCoroutine = StartCoroutine(ClearFeedbackText());
    }

    private IEnumerator ClearFeedbackText()
    {
        // Wait for 0.15 seconds
        yield return new WaitForSeconds(1f);

        // Clear the feedback text
        feedbackText.text = "";
    }


    private void CheckMissedBeats(float previousTime, float currentTime)
    {

        if (!canCheck || !canCheckMissedBeats)
            return;

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
                    SetFeedback("Beat Miss!", Color.red);
                    // Debug.Log($"Beat missed at time: {beatTime}");
                    MarkBeatAsHandled(i, false);
                    OnBeatMissed.Invoke();
                    OnBeatOccurred.Invoke();
                }
            }
        }
    }

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

    private void MarkBeatAsHandled(int index, bool isHit)
    {
        beatStatus[index] = isHit ? 1 : -1;
    }
}
