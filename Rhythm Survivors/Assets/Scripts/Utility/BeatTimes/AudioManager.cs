using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip audioClip;
	public float delayBeforeStart = 2f;
	public bool IsPlaying { get; private set; } = false;
	public double SongStartTime { get; private set; }

	/// <summary>
	/// Sets up the audio source and schedules playback.
	/// </summary>
	public void SetupAudio()
	{
		if (audioClip == null)
		{
			Debug.LogError("AudioClip not assigned!");
			return;
		}

		audioSource.clip = audioClip;
		audioSource.loop = true;
		StartCoroutine(StartAudioAfterDelay());
	}

	/// <summary>
	/// Coroutine to start audio playback after a delay.
	/// </summary>
	/// <returns>IEnumerator for coroutine.</returns>
	private IEnumerator StartAudioAfterDelay()
	{
		SongStartTime = AudioSettings.dspTime + delayBeforeStart;
		audioSource.PlayScheduled(SongStartTime);
		IsPlaying = true;
		yield return null;
	}

	/// <summary>
	/// Retrieves the current song time based on DSP time.
	/// </summary>
	/// <returns>Current song time in seconds.</returns>
	public float GetSongTime()
	{
		if (!IsPlaying) return 0f;
		float songTime = (float)(AudioSettings.dspTime - SongStartTime);
		return songTime % audioClip.length;
	}
}
