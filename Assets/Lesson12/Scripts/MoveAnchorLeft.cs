using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class AnchorToLeft : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    [ContextMenu("Set Anchor to Left")]
    public void SetAnchorToLeft()
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform not found!");
            return;
        }

        // Set the anchor to the left (0 for min and max)
        rectTransform.anchorMin = new Vector2(0f, rectTransform.anchorMin.y);
        rectTransform.anchorMax = new Vector2(0f, rectTransform.anchorMax.y);

        // Optionally reset the position so it stays in place
        rectTransform.anchoredPosition = new Vector2(0f, rectTransform.anchoredPosition.y);

        Debug.Log("Anchor set to the left for: " + gameObject.name);
    }
}