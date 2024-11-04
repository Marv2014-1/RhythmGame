using UnityEngine;
using UnityEngine.UI; // For Image
using TMPro; // For TextMeshPro

public class TransparentBackground : MonoBehaviour
{
    public Image backgroundImage;
    public float transparency = 0.5f; // Set this between 0 (fully transparent) and 1 (fully opaque)

    void Start()
    {
        SetTransparency(transparency);
    }

    public void SetTransparency(float alpha)
    {
        if (backgroundImage != null)
        {
            Color newColor = backgroundImage.color;
            newColor.a = alpha; // Set alpha to desired transparency level
            backgroundImage.color = newColor;
        }
    }
}
