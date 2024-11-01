using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip audioClip;
	public bool IsPlaying { get; private set; } = false;
	public double SongStartTime { get; private set; }

	/// Sets up the audio source without starting playback.
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
		IsPlaying = false;
	}

	/// Plays the audio, ensuring the data is loaded.
	public void PlayAudio()
	{
		if (audioClip.loadState != AudioDataLoadState.Loaded)
		{
			Debug.Log("Audio data not loaded yet, loading now...");
			audioClip.LoadAudioData();
			StartCoroutine(WaitForAudioLoadAndPlay());
			return;
		}

		audioSource.Play();
		SongStartTime = AudioSettings.dspTime;
		IsPlaying = true;
	}

	private IEnumerator WaitForAudioLoadAndPlay()
	{
		while (audioClip.loadState == AudioDataLoadState.Loading)
		{
			yield return null;
		}

		if (audioClip.loadState == AudioDataLoadState.Loaded)
		{
			audioSource.Play();
			SongStartTime = AudioSettings.dspTime;
			IsPlaying = true;
		}
		else
		{
			Debug.LogError("Failed to load audio data.");
		}
	}

	/// Retrieves the current song time based on DSP time.
	public float GetSongTime()
	{
		if (!IsPlaying) return 0f;
		float songTime = (float)(AudioSettings.dspTime - SongStartTime);
		return songTime % audioClip.length;
	}

	/// Stops the audio playback.
	public void StopAudio()
	{
		audioSource.Stop();
		IsPlaying = false;
	}
}
