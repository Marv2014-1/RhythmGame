using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip audioClip;
	public bool IsPlaying { get; private set; } = false;
	public double SongStartTime { get; private set; }

	/// <summary>
	/// Sets up the audio source and schedules playback.
	/// </summary>
	public void SetupAudio(Song song)
	{
		if (song.audioClip == null)
		{
			Debug.LogError("AudioClip not assigned!");
			return;
		}

		audioClip = song.audioClip;
		audioSource.clip = audioClip;
		audioSource.loop = false; // We'll handle looping manually
		SongStartTime = AudioSettings.dspTime;
		audioSource.PlayScheduled(SongStartTime);
		IsPlaying = true;
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

	/// <summary>
	/// Stops the audio playback.
	/// </summary>
	public void StopAudio()
	{
		audioSource.Stop();
		IsPlaying = false;
	}

	/// <summary>
	/// Plays the audio immediately.
	/// </summary>
	public void PlayAudio()
	{
		audioSource.Play();
		SongStartTime = AudioSettings.dspTime;
		IsPlaying = true;
	}
}
