using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BeatTimeLoader
{
	public List<float> LoadBeatTimes(string subfolder, string jsonFileName, float delay)
	{
		TextAsset jsonFile = Resources.Load<TextAsset>($"{subfolder}/{jsonFileName}");

		if (jsonFile != null)
		{
			Debug.Log($"JSON file loaded successfully. Content: {jsonFile.text}");
			List<float> parsedBeatTimes = ManuallyParseBeatTimes(jsonFile.text, delay);

			if (parsedBeatTimes != null && parsedBeatTimes.Count > 0)
			{
				return parsedBeatTimes.OrderBy(bt => bt).ToList();
			}
			else
			{
				Debug.LogError("Failed to parse the JSON content or no beats found.");
			}
		}
		else
		{
			Debug.LogError($"Failed to load the JSON file from Resources/{subfolder}/{jsonFileName}");
		}

		return new List<float>();
	}

	private List<float> ManuallyParseBeatTimes(string jsonText, float delay)
	{
		List<float> beatTimeList = new List<float>();
		string key = "\"BeatTimes\"";
		int keyIndex = jsonText.IndexOf(key);

		if (keyIndex == -1)
		{
			Debug.LogError("\"BeatTimes\" key not found in JSON.");
			return null;
		}

		int arrayStart = jsonText.IndexOf('[', keyIndex);
		if (arrayStart == -1)
		{
			Debug.LogError("Start of array '[' not found after \"BeatTimes\" key.");
			return null;
		}

		int arrayEnd = jsonText.IndexOf(']', arrayStart);
		if (arrayEnd == -1)
		{
			Debug.LogError("End of array ']' not found after \"BeatTimes\" key.");
			return null;
		}

		string arrayContent = jsonText.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
		string[] numberStrings = arrayContent.Split(',');

		foreach (string numStr in numberStrings)
		{
			string trimmedStr = numStr.Trim();
			if (float.TryParse(trimmedStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float beatTime))
			{
				beatTimeList.Add(beatTime + delay);
			}
			else
			{
				Debug.LogWarning($"Failed to parse beat time: {trimmedStr}");
			}
		}

		return beatTimeList;
	}
}
