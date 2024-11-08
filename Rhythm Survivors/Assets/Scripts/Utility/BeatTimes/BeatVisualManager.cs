using UnityEngine;
using System.Collections.Generic;

public class BeatVisualManager : MonoBehaviour
{
	public RectTransform beatVisualContainer;
	public GameObject beatVisualPrefab;
	public float visualDuration = 5f;

	private AudioManager audioManager;
	private List<BeatVisual> activeBeatVisuals = new List<BeatVisual>();

	private float visualStartTime = 0f; // Time when visuals started updating
	private bool visualsStarted = false;

	void Awake()
	{
		audioManager = FindObjectOfType<BeatDetector>().GetComponent<AudioManager>();
	}

	/// <summary>
	/// Initializes beat visuals for beats that are within the visual duration from the current time.
	/// </summary>
	/// <param name="beatTimes">List of all beat times.</param>
	public void InitializeBeatVisuals()
	{
		// Clear existing visuals
		ClearAllBeatVisuals();

		// Reset visual start time
		visualStartTime = Time.time;
		visualsStarted = true;
	}

	/// <summary>
	/// Updates the positions of active beat visuals and manages their lifecycle.
	/// </summary>
	/// <param name="beatTimes">List of all beat times.</param>
	public void UpdateBeatVisuals(List<float> beatTimes)
	{
		if (!visualsStarted) return;

		float visualTime = Time.time - visualStartTime; // Time since visuals started

		var visualsToRemove = new List<BeatVisual>();

		foreach (BeatVisual beatVisual in activeBeatVisuals)
		{
			float timeUntilBeat = beatVisual.beatTime - visualTime;

			if (timeUntilBeat <= 0f)
			{
				// Beat has occurred, remove the visual
				Destroy(beatVisual.rectTransform.gameObject);
				visualsToRemove.Add(beatVisual);
			}
			else if (timeUntilBeat <= visualDuration)
			{
				// Calculate progress
				float progress = timeUntilBeat / visualDuration;

				float halfWidth = beatVisualContainer.rect.width / 2;
				float startX = beatVisual.fromLeft ? -halfWidth : halfWidth;
				float xPosition = Mathf.Lerp(0f, startX, progress);

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
			float timeUntilBeat = beatTime - visualTime;

			if (timeUntilBeat >= 0f && timeUntilBeat <= visualDuration)
			{
				// Check if visuals from both sides exist
				bool leftVisualExists = activeBeatVisuals.Exists(bv => Mathf.Approximately(bv.beatTime, beatTime) && bv.fromLeft == true);
				bool rightVisualExists = activeBeatVisuals.Exists(bv => Mathf.Approximately(bv.beatTime, beatTime) && bv.fromLeft == false);

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
		Vector2 desiredSize = new Vector2(30f, 100f);

		GameObject beatVisualObject = Instantiate(beatVisualPrefab, beatVisualContainer);
		RectTransform rectTransform = beatVisualObject.GetComponent<RectTransform>();
		SetupBeatVisualRectTransform(rectTransform, desiredSize, fromLeft);

		// Set initial position at the edge
		float halfWidth = beatVisualContainer.rect.width / 2;
		float startX = fromLeft ? -halfWidth : halfWidth;
		rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);

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
		rectTransform.pivot = new Vector2(0.5f, 0f);
		rectTransform.anchorMin = new Vector2(0.5f, 0f);
		rectTransform.anchorMax = new Vector2(0.5f, 0f);
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
		visualsStarted = false;
	}
}
