using UnityEngine;
using System.Collections.Generic;

public class BeatVisualManager : MonoBehaviour
{
	public RectTransform beatVisualContainer;
	public GameObject beatVisualPrefab;
	public float visualDuration = 5f;

	private AudioManager audioManager;
	private List<BeatVisual> activeBeatVisuals = new List<BeatVisual>();

	void Awake()
	{
		audioManager = FindObjectOfType<BeatDetector>().GetComponent<AudioManager>();
	}

	/// <summary>
	/// Initializes beat visuals for beats that are within the visual duration from the current song time.
	/// </summary>
	/// <param name="beatTimes">List of all beat times.</param>
	/// <param name="songTime">Current song time.</param>
	public void InitializeBeatVisuals(List<float> beatTimes, float songTime)
	{
		// Clear existing visuals
		foreach (BeatVisual beatVisual in activeBeatVisuals)
		{
			Destroy(beatVisual.rectTransform.gameObject);
		}
		activeBeatVisuals.Clear();

		foreach (float beatTime in beatTimes)
		{
			float timeUntilBeat = beatTime - songTime;
			if (timeUntilBeat < 0f)
			{
				timeUntilBeat += audioManager.audioClip.length;
			}

			if (timeUntilBeat >= 0f && timeUntilBeat <= visualDuration)
			{
				// Create visuals for both left and right sides
				CreateBeatVisual(beatTime, true);  // From Left
				CreateBeatVisual(beatTime, false); // From Right
			}
		}
	}

	/// <summary>
	/// Updates the positions of active beat visuals and manages their lifecycle.
	/// </summary>
	/// <param name="beatTimes">List of all beat times.</param>
	/// <param name="songTime">Current song time.</param>
	public void UpdateBeatVisuals(List<float> beatTimes, float songTime)
	{
		float audioClipLength = audioManager.audioClip.length;
		var visualsToRemove = new List<BeatVisual>();

		foreach (BeatVisual beatVisual in activeBeatVisuals)
		{
			float timeUntilBeat = beatVisual.beatTime - songTime;

			// Handle looping
			if (timeUntilBeat < -audioClipLength / 2)
			{
				timeUntilBeat += audioClipLength;
			}
			else if (timeUntilBeat > audioClipLength / 2)
			{
				timeUntilBeat -= audioClipLength;
			}

			if (timeUntilBeat <= 0f)
			{
				// Beat has occurred, remove the visual
				Destroy(beatVisual.rectTransform.gameObject);
				visualsToRemove.Add(beatVisual);
			}
			else if (timeUntilBeat <= visualDuration)
			{
				// Visual is active, update its position
				float progress = 1f - (timeUntilBeat / visualDuration);
				float halfWidth = beatVisualContainer.rect.width / 2;
				float targetX = beatVisual.fromLeft ? halfWidth : -halfWidth;
				float xPosition = progress * targetX;
				beatVisual.rectTransform.anchoredPosition = new Vector2(xPosition, beatVisual.rectTransform.anchoredPosition.y);
			}
			else
			{
				// Beat is too far in the future, remove the visual
				Destroy(beatVisual.rectTransform.gameObject);
				visualsToRemove.Add(beatVisual);
			}
		}

		// Remove destroyed visuals from the active list
		foreach (BeatVisual beatVisual in visualsToRemove)
		{
			activeBeatVisuals.Remove(beatVisual);
		}

		// Add new visuals for upcoming beats
		foreach (float beatTime in beatTimes)
		{
			float timeUntilBeat = beatTime - songTime;

			// Handle looping
			if (timeUntilBeat < -audioClipLength / 2)
			{
				timeUntilBeat += audioClipLength;
			}
			else if (timeUntilBeat > audioClipLength / 2)
			{
				timeUntilBeat -= audioClipLength;
			}

			if (timeUntilBeat >= 0f && timeUntilBeat <= visualDuration)
			{
				// Check if visuals from both sides exist
				bool leftVisualExists = activeBeatVisuals.Exists(bv => bv.beatTime == beatTime && bv.fromLeft == true);
				bool rightVisualExists = activeBeatVisuals.Exists(bv => bv.beatTime == beatTime && bv.fromLeft == false);

				if (!leftVisualExists)
				{
					CreateBeatVisual(beatTime, true); // From Left
				}
				if (!rightVisualExists)
				{
					CreateBeatVisual(beatTime, false); // From Right
				}
			}
		}
	}


	/// <summary>
	/// Creates a beat visual from the specified direction.
	/// </summary>
	/// <param name="beatTime">The time when the beat occurs.</param>
	/// <param name="fromLeft">Direction from which the visual originates.</param>
	private void CreateBeatVisual(float beatTime, bool fromLeft)
	{
		Debug.Log($"Creating beat visual for time {beatTime} from {(fromLeft ? "Left" : "Right")}");
		Vector2 desiredSize = new Vector2(30f, 100f);

		GameObject beatVisualObject = Instantiate(beatVisualPrefab, beatVisualContainer);
		RectTransform rectTransform = beatVisualObject.GetComponent<RectTransform>();
		SetupBeatVisualRectTransform(rectTransform, desiredSize, fromLeft);
		BeatVisual beatVisual = new BeatVisual(rectTransform, beatTime, fromLeft);
		activeBeatVisuals.Add(beatVisual);
	}

	/// <summary>
	/// Configures the RectTransform for the beat visual.
	/// </summary>
	/// <param name="rectTransform">The RectTransform to configure.</param>
	/// <param name="size">Desired size of the visual.</param>
	/// <param name="fromLeft">Direction from which the visual originates.</param>
	private void SetupBeatVisualRectTransform(RectTransform rectTransform, Vector2 size, bool fromLeft)
	{
		rectTransform.sizeDelta = size;
		rectTransform.pivot = new Vector2(fromLeft ? 0f : 1f, 0f);
		rectTransform.anchorMin = new Vector2(fromLeft ? 0f : 1f, 0f);
		rectTransform.anchorMax = new Vector2(fromLeft ? 0f : 1f, 0f);
		rectTransform.anchoredPosition = new Vector2(0f, 25f);
	}

	/// <summary>
	/// Represents a beat visual with its associated properties.
	/// </summary>
	private class BeatVisual
	{
		public RectTransform rectTransform;
		public float beatTime;
		public bool fromLeft;

		public BeatVisual(RectTransform rectTransform, float beatTime, bool fromLeft)
		{
			this.rectTransform = rectTransform;
			this.beatTime = beatTime;
			this.fromLeft = fromLeft;
		}
	}

	public void SetAudioManager(AudioManager audioManager)
	{
		this.audioManager = audioManager;
	}

	public void ClearAllBeatVisuals()
	{
		foreach (BeatVisual beatVisual in activeBeatVisuals)
		{
			Destroy(beatVisual.rectTransform.gameObject);
		}
		activeBeatVisuals.Clear();
	}

}
