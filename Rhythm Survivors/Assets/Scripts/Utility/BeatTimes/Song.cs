using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Song : MonoBehaviour
{
	public AudioClip audioClip;
	public string jsonFileName;
	public int loopCount;
	public int delayOffset;
}
