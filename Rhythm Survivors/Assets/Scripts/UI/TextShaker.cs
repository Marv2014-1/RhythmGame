using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class TextShaker : MonoBehaviour
{
    // Rotation parameters
    [Header("Rotation Settings")]
    [Tooltip("Amplitude of rotation in degrees.")]
    public float rotationAmplitude = 10f;

    [Tooltip("Frequency of rotation in oscillations per second.")]
    public float rotationFrequency = 2f;

    private RectTransform rectTransform;
    private float initialZRotation;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialZRotation = rectTransform.localRotation.eulerAngles.z;
    }

    void Update()
    {
        // Calculate the new rotation angle using a sine wave
        float angle = rotationAmplitude * Mathf.Sin(Time.time * Mathf.PI * 2f * rotationFrequency);

        // Apply the rotation around the Z-axis
        Quaternion targetRotation = Quaternion.Euler(0, 0, initialZRotation + angle);
        rectTransform.localRotation = targetRotation;
    }
}
