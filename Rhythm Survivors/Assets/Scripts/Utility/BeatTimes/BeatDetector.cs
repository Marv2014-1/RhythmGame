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
    //allows the player to hit beats that have been destroyed
    private Dictionary<int, int> beatStatus = new Dictionary<int, int>();
    private float previousAudioTime = 0f;

    private BeatTimeLoader beatTimeLoader;
    private AudioManager audioManager;
    private BeatVisualManager beatVisualManager;

    public UnityEvent OnBeatHit;
    public UnityEvent OnBeatMissed;
    public UnityEvent OnBeatOccurred;

    private bool songStarted = false;

    [Header("Playlist Settings")]
    public List<Song> playlist = new List<Song>();

    private int currentSongIndex = -1;
    private int currentLoopCount = 0;

    private List<List<float>> allBeatTimes = new List<List<float>>();
    private List<List<float>> allBeatTimesWDelay = new List<List<float>>();
    public float delay = 4.0f;
    private float songStartTime = 0f;

    private bool isTransitionSong = false;
    private bool isLoopingSong = false;

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
            songStarted = false;

            // Preload all beat times
            for (int i = 0; i < playlist.Count; i++)
            {
                string jsonFileName = playlist[i].jsonFileName;
                List<float> songBeats = beatTimeLoader.LoadBeatTimes(subfolder, jsonFileName, 0);
                allBeatTimes.Add(songBeats);
                List<float> songBeatsWDelay = beatTimeLoader.LoadBeatTimes(subfolder, jsonFileName, delay);
                allBeatTimesWDelay.Add(songBeatsWDelay);
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
        if (audioSongTime < previousAudioTime)
        {
            currentLoopCount++;
            if (currentLoopCount <= playlist[currentSongIndex].loopCount)
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

        CheckMissedBeats(adjustedPreviousTime, adjustedSongTime);
        DetectPlayerInput(adjustedSongTime);

        // Update previousAudioTime with the current audio time
        previousAudioTime = audioSongTime;
    }



    private IEnumerator TransitionToNextSong()
    {
        audioManager.StopAudio();

        currentSongIndex++;
        if (currentSongIndex >= playlist.Count)
        {
            Debug.Log("Playlist ended.");
            songStarted = false;
            beatVisualManager.ClearAllBeatVisuals();

            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            pauseMenu.EndRun();

            yield break;
        }

        currentLoopCount = 0;
        isTransitionSong = true;
        isLoopingSong = false;

        // Use delayed beat times when transitioning to the next song
        beatTimes = allBeatTimesWDelay[currentSongIndex];

        // Initialize beat statuses
        beatStatus.Clear();
        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // 0 = unhandled
            Debug.Log($"Beat Time (With Delay): {beatTimes[i]}");
        }

        // Initialize beat visuals with songTime = 0f
        beatVisualManager.InitializeBeatVisuals(beatTimes, 0f);

        audioManager.SetupAudio(playlist[currentSongIndex]);
        audioManager.StopAudio();
        Debug.Log($"Transitioning to next song: {playlist[currentSongIndex].audioClip.name}");

        songStartTime = Time.time - delay;
        songStarted = true;

        // Wait for the specified delay
        Debug.Log($"Waiting for {delay} seconds before starting the song.");
        yield return new WaitForSeconds(delay);

        // Start the audio and set songStarted to true
        audioManager.PlayAudio();
        Debug.Log("Song has started playing.");
    }

    private IEnumerator OnSongLooped()
    {
        isTransitionSong = false;
        isLoopingSong = true;

        // Use delayed beat times when the song loops
        beatTimes = allBeatTimesWDelay[currentSongIndex];

        // Reset beat statuses
        beatStatus.Clear();
        for (int i = 0; i < beatTimes.Count; i++)
        {
            beatStatus[i] = 0; // Reset all beat statuses to unhandled
            Debug.Log($"Beat Time (With Delay): {beatTimes[i]}");
        }

        // Reset songStartTime and previousAudioTime
        songStartTime = Time.time - delay;
        previousAudioTime = 0f;

        // Re-initialize beat visuals with songTime = 0f
        beatVisualManager.InitializeBeatVisuals(beatTimes, 0f);
        Debug.Log("Song has looped. Variables have been reset.");

        // Wait for the specified delay
        Debug.Log($"Waiting for {delay} seconds before restarting the song.");
        yield return new WaitForSeconds(delay);

        // Ensure audio starts from the beginning
        audioManager.audioSource.time = 0f;

        // Start the audio
        audioManager.PlayAudio();
        Debug.Log("Song has started playing from the beginning.");

        songStarted = true;
    }





    private void DetectPlayerInput(float songTime)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckBeatAccuracy(songTime);
            Debug.Log("Hit Time: " + songTime);
        }
    }


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
        //print hit time and closest beat time
        print("Hit Time: " + inputTime + " Closest Beat Time: " + beatTimes[index]);
    }

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
