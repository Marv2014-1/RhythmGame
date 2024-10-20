using UnityEngine;

public class HealthBarPositioner : MonoBehaviour
{
    [Header("Position Settings")]
    public Vector2 offset = new Vector2(20f, -20f); // Offset from the top-left corner
    public Vector2 anchorMin = new Vector2(0, 1);    // Top-left corner
    public Vector2 anchorMax = new Vector2(0, 1);    // Top-left corner
    public Vector2 pivot = new Vector2(0, 1);        // Pivot at top-left

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        PositionHealthBar();
    }

    void PositionHealthBar()
    {
        // Set the anchors
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;

        // Set the anchored position based on the offset
        rectTransform.anchoredPosition = offset;
    }

}
